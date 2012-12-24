
//Enable this ONLY if you upgrade the Windows Phone project to 7.1!
//#define MANGO

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

#if WINDOWS_PHONE
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework.Input.Touch;
#endif

public class MonkeyConfig{
//${CONFIG_BEGIN}
public const String BINARY_FILES="*.bin|*.dat";
public const String CD="";
public const String CONFIG="debug";
public const String HOST="winnt";
public const String IMAGE_FILES="*.png|*.jpg";
public const String LANG="cs";
public const String MODPATH=".;C:/Users/Chris/Documents/GitHub/CM-YATY;C:/apps/MonkeyPro66/modules";
public const String MOJO_AUTO_SUSPEND_ENABLED="0";
public const String MOJO_BACKBUFFER_ACCESS_ENABLED="1";
public const String MOJO_IMAGE_FILTERING_ENABLED="0";
public const String MUSIC_FILES="*.mp3|*.wma";
public const String SAFEMODE="0";
public const String SOUND_FILES="*.wav";
public const String TARGET="xna";
public const String TEXT_FILES="*.txt|*.xml|*.json";
public const String TRANSDIR="";
public const String XNA_ACCELEROMETER_ENABLED="1";
public const String XNA_VSYNC_ENABLED="0";
public const String XNA_WINDOW_FULLSCREEN="0";
public const String XNA_WINDOW_FULLSCREEN_PHONE="1";
public const String XNA_WINDOW_FULLSCREEN_XBOX="1";
public const String XNA_WINDOW_HEIGHT="720";
public const String XNA_WINDOW_HEIGHT_PHONE="800";
public const String XNA_WINDOW_HEIGHT_XBOX="480";
public const String XNA_WINDOW_RESIZABLE="0";
public const String XNA_WINDOW_WIDTH="480";
public const String XNA_WINDOW_WIDTH_PHONE="480";
public const String XNA_WINDOW_WIDTH_XBOX="640";
//${CONFIG_END}
}

public class MonkeyData{

	public static FileStream OpenFile( String path,FileMode mode ){
		if( path.StartsWith("monkey://internal/") ){
#if WINDOWS
			IsolatedStorageFile file=IsolatedStorageFile.GetUserStoreForAssembly();
#else
			IsolatedStorageFile file=IsolatedStorageFile.GetUserStoreForApplication();
#endif
			if( file==null ) return null;
			
			IsolatedStorageFileStream stream=file.OpenFile( path.Substring(18),mode );
			
			return stream;
		}
		return null;
	}

	public static String ContentPath( String path ){
		if( path.StartsWith("monkey://data/") ) return "Content/monkey/"+path.Substring(14);
		return "";
	}

	public static byte[] loadBytes( String path ){
		path=ContentPath( path );
		if( path=="" ) return null;
        try{
			Stream stream=TitleContainer.OpenStream( path );
			int len=(int)stream.Length;
			byte[] buf=new byte[len];
			int n=stream.Read( buf,0,len );
			stream.Close();
			if( n==len ) return buf;
		}catch( Exception ){
		}
		return null;
	}
	
	public static String LoadString( String path ){
		path=ContentPath( path );
		if( path=="" ) return "";
        try{
			Stream stream=TitleContainer.OpenStream( path );
			StreamReader reader=new StreamReader( stream );
			String text=reader.ReadToEnd();
			reader.Close();
			return text;
		}catch( Exception ){
		}
		return "";
	}
	
	public static Texture2D LoadTexture2D( String path,ContentManager content ){
		path=ContentPath( path );
		if( path=="" ) return null;
		try{
			return content.Load<Texture2D>( path );
		}catch( Exception ){
		}
		return null;
	}

	public static SoundEffect LoadSoundEffect( String path,ContentManager content ){
		path=ContentPath( path );
		if( path=="" ) return null;
		try{
			return content.Load<SoundEffect>( path );
		}catch( Exception ){
		}
		return null;
	}
	
	public static Song LoadSong( String path,ContentManager content ){
		path=ContentPath( path );
		if( path=="" ) return null;
		try{
			return content.Load<Song>( path );
		}catch( Exception ){
		}
		return null;
	}
	
};

//${TRANSCODE_BEGIN}

// C# Monkey runtime.
//
// Placed into the public domain 24/02/2011.
// No warranty implied; use at your own risk.

//using System;
//using System.Collections;

public class bb_std_lang{

	public static String errInfo="";
	public static List<String> errStack=new List<String>();
	
	public const float D2R=0.017453292519943295f;
	public const float R2D=57.29577951308232f;
	
	public static void pushErr(){
		errStack.Add( errInfo );
	}
	
	public static void popErr(){
		errInfo=errStack[ errStack.Count-1 ];
		errStack.RemoveAt( errStack.Count-1 );
	}

	public static String StackTrace(){
		if( errInfo.Length==0 ) return "";
		String str=errInfo+"\n";
		for( int i=errStack.Count-1;i>0;--i ){
			str+=errStack[i]+"\n";
		}
		return str;
	}
	
	public static int Print( String str ){
		Console.WriteLine( str );
		return 0;
	}
	
	public static int Error( String str ){
		throw new Exception( str );
	}
	
	public static int DebugLog( String str ){
		Print( str );
		return 0;
	}
	
	public static int DebugStop(){
		Error( "STOP" );
		return 0;
	}
	
	public static void PrintError( String err ){
		if( err.Length==0 ) return;
		Print( "Monkey Runtime Error : "+err );
		Print( "" );
		Print( StackTrace() );
	}
	
	//***** String stuff *****
	
	static public String[] stringArray( int n ){
		String[] t=new String[n];
		for( int i=0;i<n;++i ) t[i]="";
		return t;
	}
	
	static public String slice( String str,int from ){
		return slice( str,from,str.Length );
	}
	
	static public String slice( String str,int from,int term ){
		int len=str.Length;
		if( from<0 ){
			from+=len;
			if( from<0 ) from=0;
		}else if( from>len ){
			from=len;
		}
		if( term<0 ){
			term+=len;
		}else if( term>len ){
			term=len;
		}
		if( term>from ) return str.Substring( from,term-from );
		return "";
	}

	static public String[] split( String str,String sep ){
		if( sep.Length==0 ){
			String[] bits=new String[str.Length];
			for( int i=0;i<str.Length;++i ){
				bits[i]=new String( str[i],1 );
			}
			return bits;
		}else{
			int i=0,i2,n=1;
			while( (i2=str.IndexOf( sep,i ))!=-1 ){
				++n;
				i=i2+sep.Length;
			}
			String[] bits=new String[n];
			i=0;
			for( int j=0;j<n;++j ){
				i2=str.IndexOf( sep,i );
				if( i2==-1 ) i2=str.Length;
				bits[j]=slice( str,i,i2 );
				i=i2+sep.Length;
			}
			return bits;
		}
	}
	
	static public String fromChars( int[] chars ){
		int n=chars.Length;
		char[] chrs=new char[n];
		for( int i=0;i<n;++i ){
			chrs[i]=(char)chars[i];
		}
		return new String( chrs,0,n );
	}
	
	//***** Array stuff *****
	
	static public Array slice( Array arr,int from ){
		return slice( arr,from,arr.Length );
	}
	
	static public Array slice( Array arr,int from,int term ){
		int len=arr.Length;
		if( from<0 ){
			from+=len;
			if( from<0 ) from=0;
		}else if( from>len ){
			from=len;
		}
		if( term<0 ){
			term+=len;
		}else if( term>len ){
			term=len;
		}
		if( term<from ) term=from;
		int newlen=term-from;
		Array res=Array.CreateInstance( arr.GetType().GetElementType(),newlen );
		if( newlen>0 ) Array.Copy( arr,from,res,0,newlen );
		return res;
	}

	static public Array resizeArray( Array arr,int len ){
		Type ty=arr.GetType().GetElementType();
		Array res=Array.CreateInstance( ty,len );
		int n=Math.Min( arr.Length,len );
		if( n>0 ) Array.Copy( arr,res,n );
		return res;
   }

	static public Array[] resizeArrayArray( Array[] arr,int len ){
		int i=arr.Length;
		arr=(Array[])resizeArray( arr,len );
		if( i<len ){
			Array empty=Array.CreateInstance( arr.GetType().GetElementType().GetElementType(),0 );
			while( i<len ) arr[i++]=empty;
		}
		return arr;
	}

	static public String[] resizeStringArray( String[] arr,int len ){
		int i=arr.Length;
		arr=(String[])resizeArray( arr,len );
		while( i<len ) arr[i++]="";
		return arr;
	}
	
	static public Array concat( Array lhs,Array rhs ){
		Array res=Array.CreateInstance( lhs.GetType().GetElementType(),lhs.Length+rhs.Length );
		Array.Copy( lhs,0,res,0,lhs.Length );
		Array.Copy( rhs,0,res,lhs.Length,rhs.Length );
		return res;
	}
	
	static public int length( Array arr ){
		return arr!=null ? arr.Length : 0;
	}
	
	static public int[] toChars( String str ){
		int[] arr=new int[str.Length];
		for( int i=0;i<str.Length;++i ) arr[i]=(int)str[i];
		return arr;
	}
}

class ThrowableObject : Exception{
	public ThrowableObject() : base( "Uncaught Monkey Exception" ){
	}
};

public class BBDataBuffer{

    public byte[] _data;
    public int _length;
    
    public BBDataBuffer(){
    }

    public BBDataBuffer( int length ){
    	_data=new byte[length];
    	_length=length;
    }
    
    public BBDataBuffer( byte[] data ){
    	_data=data;
    	_length=data.Length;
    }
    
    public bool _New( int length ){
    	if( _data!=null ) return false;
    	_data=new byte[length];
    	_length=length;
    	return true;
    }
    
    public bool _Load( String path ){
    	if( _data!=null ) return false;
    	
    	_data=MonkeyData.loadBytes( path );
    	if( _data==null ) return false;
    	
    	_length=_data.Length;
    	return true;
    }
    
    public void Discard(){
    	if( _data!=null ){
    		_data=null;
    		_length=0;
    	}
    }
    
  	public int Length(){
  		return _length;
  	}

	public void PokeByte( int addr,int value ){
		_data[addr]=(byte)value;
	}

	public void PokeShort( int addr,int value ){
		Array.Copy( System.BitConverter.GetBytes( (short)value ),0,_data,addr,2 );
	}

	public void PokeInt( int addr,int value ){
		Array.Copy( System.BitConverter.GetBytes( value ),0,_data,addr,4 );
	}

	public void PokeFloat( int addr,float value ){
		Array.Copy( System.BitConverter.GetBytes( value ),0,_data,addr,4 );
	}

	public int PeekByte( int addr ){
		return (int)(sbyte)_data[addr];
	}

	public int PeekShort( int addr ){
		return (int)System.BitConverter.ToInt16( _data,addr );
	}

	public int PeekInt( int addr ){
		return System.BitConverter.ToInt32( _data,addr );
	}

	public float PeekFloat( int addr ){
		return (float)System.BitConverter.ToSingle( _data,addr );
	}
}
// XNA mojo runtime.
//
// Copyright 2011 Mark Sibly, all rights reserved.
// No warranty implied; use at your own risk.

public class gxtkGame : Game{

	public gxtkApp app;
	
	public GraphicsDeviceManager deviceManager;
	
	public bool activated,autoSuspend;

	public gxtkGame(){
	
		gxtkApp.game=this;
	
		deviceManager=new GraphicsDeviceManager( this );
		
		deviceManager.PreparingDeviceSettings+=new EventHandler<PreparingDeviceSettingsEventArgs>( PreparingDeviceSettings );
		
#if WINDOWS
		deviceManager.IsFullScreen=MonkeyConfig.XNA_WINDOW_FULLSCREEN=="1";
		deviceManager.PreferredBackBufferWidth=int.Parse( MonkeyConfig.XNA_WINDOW_WIDTH );
		deviceManager.PreferredBackBufferHeight=int.Parse( MonkeyConfig.XNA_WINDOW_HEIGHT );
		Window.AllowUserResizing=MonkeyConfig.XNA_WINDOW_RESIZABLE=="1";
#elif XBOX
		deviceManager.IsFullScreen=MonkeyConfig.XNA_WINDOW_FULLSCREEN_XBOX=="1";
		deviceManager.PreferredBackBufferWidth=int.Parse( MonkeyConfig.XNA_WINDOW_WIDTH_XBOX );
		deviceManager.PreferredBackBufferHeight=int.Parse( MonkeyConfig.XNA_WINDOW_HEIGHT_XBOX );
#elif WINDOWS_PHONE
		deviceManager.IsFullScreen=MonkeyConfig.XNA_WINDOW_FULLSCREEN_PHONE=="1";
		deviceManager.PreferredBackBufferWidth=int.Parse( MonkeyConfig.XNA_WINDOW_WIDTH_PHONE );
		deviceManager.PreferredBackBufferHeight=int.Parse( MonkeyConfig.XNA_WINDOW_HEIGHT_PHONE );
#endif
		IsMouseVisible=true;

		autoSuspend=MonkeyConfig.MOJO_AUTO_SUSPEND_ENABLED=="1";
	}
	
	void PreparingDeviceSettings( Object sender,PreparingDeviceSettingsEventArgs e ){
		if( MonkeyConfig.XNA_VSYNC_ENABLED=="1" ){
			PresentationParameters pp=e.GraphicsDeviceInformation.PresentationParameters;
			pp.PresentationInterval=PresentInterval.One;
		}
	}
	
	void CheckActive(){
		//wait for first activation		
		if( !activated ){
			activated=IsActive;
			return;
		}
		
		if( autoSuspend ){
			if( IsActive ){
				app.InvokeOnResume();
			}else{
				app.InvokeOnSuspend();
			}
		}else{
			if( Window.ClientBounds.Width>0 && Window.ClientBounds.Height>0 ){
				app.InvokeOnResume();
			}else{
				app.InvokeOnSuspend();
			}
		}
	}

	protected override void LoadContent(){
		try{

			bb_.bbInit();
			bb_.bbMain();
			
			if( app!=null ) app.InvokeOnCreate();
			
		}catch( Exception ex ){
			Die( ex );
		}
	}
	
	protected override void Update( GameTime gameTime ){
		if( app==null ) return;
		
		CheckActive();
		
		app.Update( gameTime );

		base.Update( gameTime );
	}
	
	protected override bool BeginDraw(){
		return app!=null && !app.suspended && base.BeginDraw();
	}

	protected override void Draw( GameTime gameTime ){
		if( app==null ) return;
		
		CheckActive();
		
		app.Draw( gameTime );

		base.Draw( gameTime );
	}
	
#if !WINDOWS_PHONE
	public static void Main(){
		new gxtkGame().Run();
	}
#endif
	
	public void Die( Exception ex ){
		bb_std_lang.PrintError( ex.Message );
		Exit();
	}
}

public class gxtkApp{

	public static gxtkGame game;

	public gxtkGraphics graphics;
	public gxtkInput input;
	public gxtkAudio audio;
	
	public int updateRate;
	public double nextUpdate;
	public double updatePeriod;
	
#if WINDOWS	
	public Stopwatch stopwatch;
#else	
	public int tickcount;
#endif
	
	public bool suspended;
	
	public gxtkApp(){

		game.app=this;

		graphics=new gxtkGraphics();
		input=new gxtkInput();
		audio=new gxtkAudio();
		
		game.TargetElapsedTime=TimeSpan.FromSeconds( 1.0/10.0 );

#if WINDOWS
		stopwatch=Stopwatch.StartNew();
#else		
		tickcount=System.Environment.TickCount;
#endif
	}

	public void Die( Exception ex ){
		game.Die( ex );
	}
	
	public void Update( GameTime gameTime ){
	
		int updates=0;
		
		for(;;){
		
			nextUpdate+=updatePeriod;
			
			InvokeOnUpdate();
			
			if( !game.IsFixedTimeStep || updateRate==0 ) break;
			
			if( nextUpdate>(double)game.app.MilliSecs() ) break;
			
			if( ++updates==8 ){
				nextUpdate=(double)MilliSecs();
				break;
			}
		}
	}

	public void Draw( GameTime gameTime ){
		InvokeOnRender();
	}
	
	public void InvokeOnCreate(){
		try{
			OnCreate();
		}catch( Exception ex ){
			Die( ex );
		}
	}

	public void InvokeOnUpdate(){
		if( suspended || updateRate==0 ) return;
		
		try{
			input.BeginUpdate();
			OnUpdate();
			input.EndUpdate();
		}catch( Exception ex ){
			Die( ex );
		}
	}

	public void InvokeOnRender(){
		if( suspended ) return;
		
		try{
			graphics.BeginRender();
			OnRender();
			graphics.EndRender();
		}catch( Exception ex ){
			Die( ex );
		}
	}

	public void InvokeOnSuspend(){
		if( suspended ) return;
		
		try{
			suspended=true;
			OnSuspend();
			audio.OnSuspend();
		}catch( Exception ex ){
			Die( ex );
		}
	}

	public void InvokeOnResume(){
		if( !suspended ) return;
		
		try{
			audio.OnResume();
			OnResume();
			suspended=false;
		}catch( Exception ex ){
			Die( ex );
		}
	}

	//***** GXTK API *****
	
	public virtual gxtkGraphics GraphicsDevice(){
		return graphics;
	}
	
	public virtual gxtkInput InputDevice(){
		return input;
	}
	
	public virtual gxtkAudio AudioDevice(){
		return audio;
	}

	public virtual String AppTitle(){
		return "gxtkApp";
	}
	
	public virtual String LoadState(){
#if WINDOWS
		IsolatedStorageFile file=IsolatedStorageFile.GetUserStoreForAssembly();
#else
		IsolatedStorageFile file=IsolatedStorageFile.GetUserStoreForApplication();
#endif
		if( file==null ) return "";
		
		IsolatedStorageFileStream stream=file.OpenFile( ".mojostate",FileMode.OpenOrCreate );
		if( stream==null ){
			return "";
		}

		StreamReader reader=new StreamReader( stream );
		String state=reader.ReadToEnd();
		reader.Close();
		
		return state;
	}
	
	public virtual int SaveState( String state ){
#if WINDOWS
		IsolatedStorageFile file=IsolatedStorageFile.GetUserStoreForAssembly();
#else
		IsolatedStorageFile file=IsolatedStorageFile.GetUserStoreForApplication();
#endif
		if( file==null ) return -1;
		
		IsolatedStorageFileStream stream=file.OpenFile( ".mojostate",FileMode.Create );
		if( stream==null ){
			return -1;
		}

		StreamWriter writer=new StreamWriter( stream );
		writer.Write( state );
		writer.Close();
		
		return 0;
	}
	
	public virtual String LoadString( String path ){
		return MonkeyData.LoadString( path );
	}
	
	public virtual int SetUpdateRate( int hertz ){
		updateRate=hertz;

		if( updateRate!=0 ){

			updatePeriod=1000.0/(double)hertz;
			nextUpdate=(double)MilliSecs()+updatePeriod;

			game.TargetElapsedTime=TimeSpan.FromTicks( (long)(10000000.0/(double)hertz+.5) );

			game.IsFixedTimeStep=(MonkeyConfig.XNA_VSYNC_ENABLED!="1");

		}else{
		
			game.TargetElapsedTime=TimeSpan.FromSeconds( 1.0/10.0 );
			
		}
		return 0;
	}
	
	public virtual int MilliSecs(){
#if WINDOWS	
		return  (int)stopwatch.ElapsedMilliseconds;
#else
		return System.Environment.TickCount-tickcount;
#endif
	}

	public virtual int Loading(){
		return 0;
	}

	public virtual int OnCreate(){
		return 0;
	}

	public virtual int OnSuspend(){
		return 0;
	}

	public virtual int OnResume(){
		return 0;
	}

	public virtual int OnUpdate(){
		return 0;
	}

	public virtual int OnRender(){
		return 0;
	}

	public virtual int OnLoading(){
		return 0;
	}
}

public class gxtkGraphics{

	public const int MAX_VERTS=1024;
	public const int MAX_LINES=MAX_VERTS/2;
	public const int MAX_QUADS=MAX_VERTS/4;

	public GraphicsDevice device;
	
	RenderTarget2D renderTarget;
	
	RasterizerState rstateScissor;
	Rectangle scissorRect;
	
	BasicEffect effect;
	
	int primType;
	int primCount;
	Texture2D primTex;

	VertexPositionColorTexture[] vertices;
	Int16[] quadIndices;
	Int16[] fanIndices;

	Color color;
	
	BlendState defaultBlend;
	BlendState additiveBlend;
	
	bool tformed=false;
	float ix,iy,jx,jy,tx,ty;
	
	public gxtkGraphics(){
	
		device=gxtkApp.game.GraphicsDevice;
		
		effect=new BasicEffect( device );
		effect.VertexColorEnabled=true;

		vertices=new VertexPositionColorTexture[ MAX_VERTS ];
		for( int i=0;i<MAX_VERTS;++i ){
			vertices[i]=new VertexPositionColorTexture();
		}
		
		quadIndices=new Int16[ MAX_QUADS * 6 ];
		for( int i=0;i<MAX_QUADS;++i ){
			quadIndices[i*6  ]=(short)(i*4);
			quadIndices[i*6+1]=(short)(i*4+1);
			quadIndices[i*6+2]=(short)(i*4+2);
			quadIndices[i*6+3]=(short)(i*4);
			quadIndices[i*6+4]=(short)(i*4+2);
			quadIndices[i*6+5]=(short)(i*4+3);
		}
		
		fanIndices=new Int16[ MAX_VERTS * 3 ];
		for( int i=0;i<MAX_VERTS;++i ){
			fanIndices[i*3  ]=(short)(0);
			fanIndices[i*3+1]=(short)(i+1);
			fanIndices[i*3+2]=(short)(i+2);
		}

		rstateScissor=new RasterizerState();
		rstateScissor.CullMode=CullMode.None;
		rstateScissor.ScissorTestEnable=true;
		
		defaultBlend=BlendState.NonPremultiplied;
		
		//note: ColorSourceBlend must == AlphaSourceBlend in Reach profile!
		additiveBlend=new BlendState();
		additiveBlend.ColorBlendFunction=BlendFunction.Add;
		additiveBlend.ColorSourceBlend=Blend.SourceAlpha;
		additiveBlend.AlphaSourceBlend=Blend.SourceAlpha;
		additiveBlend.ColorDestinationBlend=Blend.One;
		additiveBlend.AlphaDestinationBlend=Blend.One;
	}

	public void BeginRender(){
	
		device.RasterizerState=RasterizerState.CullNone;
		device.DepthStencilState=DepthStencilState.None;
		device.BlendState=BlendState.NonPremultiplied;
		
		if( MonkeyConfig.MOJO_IMAGE_FILTERING_ENABLED=="1" ){
			device.SamplerStates[0]=SamplerState.LinearClamp;
		}else{
			device.SamplerStates[0]=SamplerState.PointClamp;
		}
		
		if( MonkeyConfig.MOJO_BACKBUFFER_ACCESS_ENABLED=="1" ){
			if( renderTarget!=null && (renderTarget.Width!=Width() || renderTarget.Height!=Height()) ){
				renderTarget.Dispose();
				renderTarget=null;
			}
			if( renderTarget==null ){
				renderTarget=new RenderTarget2D( device,Width(),Height() );
			}
		}
		device.SetRenderTarget( renderTarget );
		
		effect.Projection=Matrix.CreateOrthographicOffCenter( +.5f,Width()+.5f,Height()+.5f,+.5f,0,1 );

		primCount=0;
	}

	public void EndRender(){
		Flush();
		
		if( renderTarget==null ) return;
		
		device.SetRenderTarget( null );
		
		device.BlendState=BlendState.Opaque;
		
		primType=4;
		primTex=renderTarget;
		
		float x=0,y=0;
		float w=Width();
		float h=Height();
		float u0=0,u1=1,v0=0,v1=1;
		float x0=x,x1=x+w,x2=x+w,x3=x;
		float y0=y,y1=y,y2=y+h,y3=y+h;
		
		Color color=Color.White;

		int vp=primCount++*4;
		vertices[vp  ].Position.X=x0;vertices[vp  ].Position.Y=y0;
		vertices[vp  ].TextureCoordinate.X=u0;vertices[vp  ].TextureCoordinate.Y=v0;
		vertices[vp  ].Color=color;
		vertices[vp+1].Position.X=x1;vertices[vp+1].Position.Y=y1;
		vertices[vp+1].TextureCoordinate.X=u1;vertices[vp+1].TextureCoordinate.Y=v0;
		vertices[vp+1 ].Color=color;
		vertices[vp+2].Position.X=x2;vertices[vp+2].Position.Y=y2;
		vertices[vp+2].TextureCoordinate.X=u1;vertices[vp+2].TextureCoordinate.Y=v1;
		vertices[vp+2].Color=color;
		vertices[vp+3].Position.X=x3;vertices[vp+3].Position.Y=y3;
		vertices[vp+3].TextureCoordinate.X=u0;vertices[vp+3].TextureCoordinate.Y=v1;
		vertices[vp+3].Color=color;
		
		Flush();
	}
	
	public void Flush(){
		if( primCount==0 ) return;
		
		if( primTex!=null ){
	        effect.TextureEnabled=true;
    	    effect.Texture=primTex;
		}else{
	        effect.TextureEnabled=false;
		}

        foreach( EffectPass pass in effect.CurrentTechnique.Passes ){
            pass.Apply();

            switch( primType ){
			case 2:	//lines
				device.DrawUserPrimitives<VertexPositionColorTexture>(
				PrimitiveType.LineList,
				vertices,0,primCount );
				break;
			case 4:	//quads
				device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
				PrimitiveType.TriangleList,
				vertices,0,primCount*4,
				quadIndices,0,primCount*2 );
				break;
			case 5:	//trifan
				device.DrawUserIndexedPrimitives<VertexPositionColorTexture>(
				PrimitiveType.TriangleList,
				vertices,0,primCount,
				fanIndices,0,primCount-2 );
				break;
            }
        }
		primCount=0;
	}
	
	//***** GXTK API *****
	
	public virtual int Mode(){
		return 1;
	}
	
	public virtual int Width(){
		return device.PresentationParameters.BackBufferWidth;
	}
	
	public virtual int Height(){
		return device.PresentationParameters.BackBufferHeight;
	}
	
	public virtual int Loaded(){
		return 1;
	}
	
	public virtual gxtkSurface LoadSurface__UNSAFE__( gxtkSurface surface,String path ){
		Texture2D texture=MonkeyData.LoadTexture2D( path,gxtkApp.game.Content );
		if( texture==null ) return null;
		surface.SetTexture( texture );
		return surface;
	}
	
	public virtual gxtkSurface LoadSurface( String path ){
		return LoadSurface__UNSAFE__( new gxtkSurface(),path );
	}
	
	public virtual gxtkSurface CreateSurface( int width,int height ){
		Texture2D texture=new Texture2D( device,width,height,false,SurfaceFormat.Color );
		if( texture!=null ) return new gxtkSurface( texture );
		return null;
	}
	
	public virtual int SetAlpha( float alpha ){
		color.A=(byte)(alpha * 255);
		return 0;
	}

	public virtual int SetColor( float r,float g,float b ){
		color.R=(byte)r;
		color.G=(byte)g;
		color.B=(byte)b;
		return 0;
	}
	
	public virtual int SetBlend( int blend ){
		Flush();
	
		switch( blend ){
		case 1:
			device.BlendState=additiveBlend;
			break;
		default:
			device.BlendState=defaultBlend;
			break;
		}
		return 0;
	}
	
	public virtual int SetMatrix( float ix,float iy,float jx,float jy,float tx,float ty ){
	
		tformed=( ix!=1 || iy!=0 || jx!=0 || jy!=1 || tx!=0 || ty!=0 );
		
		this.ix=ix;this.iy=iy;
		this.jx=jx;this.jy=jy;
		this.tx=tx;this.ty=ty;

		return 0;
	}
	
	public virtual int SetScissor( int x,int y,int w,int h ){
		Flush();

		int r=Math.Min( x+w,Width() );
		int b=Math.Min( y+h,Height() );
		x=Math.Max( x,0 );
		y=Math.Max( y,0 );
		if( r>x && b>y ){
			w=r-x;
			h=b-y;
		}else{
			x=y=w=h=0;
		}
		
		if( x!=0 || y!=0 || w!=Width() || h!=Height() ){
			scissorRect.X=x;
			scissorRect.Y=y;
			scissorRect.Width=w;
			scissorRect.Height=h;
			device.RasterizerState=rstateScissor;
			device.ScissorRectangle=scissorRect;
		}else{
			device.RasterizerState=RasterizerState.CullNone;
		}
		
		return 0;
	}
	
	public virtual int Cls( float r,float g,float b ){

		if( device.RasterizerState.ScissorTestEnable ){

			Rectangle sr=device.ScissorRectangle;
			float x=sr.X,y=sr.Y,w=sr.Width,h=sr.Height;
			Color color=new Color( r/255.0f,g/255.0f,b/255.0f );
			
			primType=4;
			primCount=1;
			primTex=null;

			vertices[0].Position.X=x  ;vertices[0].Position.Y=y  ;vertices[0].Color=color;
			vertices[1].Position.X=x+w;vertices[1].Position.Y=y  ;vertices[1].Color=color;
			vertices[2].Position.X=x+w;vertices[2].Position.Y=y+h;vertices[2].Color=color;
			vertices[3].Position.X=x  ;vertices[3].Position.Y=y+h;vertices[3].Color=color;
		}else{
			primCount=0;
			device.Clear( new Color( r/255.0f,g/255.0f,b/255.0f ) );
		}
		return 0;
	}

	public virtual int DrawPoint( float x,float y ){
		if( primType!=4 || primCount==MAX_QUADS || primTex!=null ){
			Flush();
			primType=4;
			primTex=null;
		}
		
		if( tformed ){
			float px=x;
			x=px * ix + y * jx + tx;
			y=px * iy + y * jy + ty;
		}

		int vp=primCount++*4;
				
		vertices[vp  ].Position.X=x;vertices[vp  ].Position.Y=y;
		vertices[vp  ].Color=color;
		vertices[vp+1].Position.X=x+1;vertices[vp+1].Position.Y=y;
		vertices[vp+1].Color=color;
		vertices[vp+2].Position.X=x+1;vertices[vp+2].Position.Y=y+1;
		vertices[vp+2].Color=color;
		vertices[vp+3].Position.X=x;vertices[vp+3].Position.Y=y+1;
		vertices[vp+3].Color=color;
		
		return 0;
	}
	
	public virtual int DrawRect( float x,float y,float w,float h ){
		if( primType!=4 || primCount==MAX_QUADS || primTex!=null ){
			Flush();
			primType=4;
			primTex=null;
		}
		
		float x0=x,x1=x+w,x2=x+w,x3=x;
		float y0=y,y1=y,y2=y+h,y3=y+h;
		
		if( tformed ){
			float tx0=x0,tx1=x1,tx2=x2,tx3=x3;
			x0=tx0 * ix + y0 * jx + tx;
			y0=tx0 * iy + y0 * jy + ty;
			x1=tx1 * ix + y1 * jx + tx;
			y1=tx1 * iy + y1 * jy + ty;
			x2=tx2 * ix + y2 * jx + tx;
			y2=tx2 * iy + y2 * jy + ty;
			x3=tx3 * ix + y3 * jx + tx;
			y3=tx3 * iy + y3 * jy + ty;
		}

		int vp=primCount++*4;
				
		vertices[vp  ].Position.X=x0;vertices[vp  ].Position.Y=y0;
		vertices[vp  ].Color=color;
		vertices[vp+1].Position.X=x1;vertices[vp+1].Position.Y=y1;
		vertices[vp+1].Color=color;
		vertices[vp+2].Position.X=x2;vertices[vp+2].Position.Y=y2;
		vertices[vp+2].Color=color;
		vertices[vp+3].Position.X=x3;vertices[vp+3].Position.Y=y3;
		vertices[vp+3].Color=color;
		
		return 0;
	}

	public virtual int DrawLine( float x0,float y0,float x1,float y1 ){
		if( primType!=2 || primCount==MAX_LINES || primTex!=null ){
			Flush();
			primType=2;
			primTex=null;
		}
		
		if( tformed ){
			float tx0=x0,tx1=x1;
			x0=tx0 * ix + y0 * jx + tx;
			y0=tx0 * iy + y0 * jy + ty;
			x1=tx1 * ix + y1 * jx + tx;
			y1=tx1 * iy + y1 * jy + ty;
		}
		
		int vp=primCount++*2;
		
		vertices[vp  ].Position.X=x0;vertices[vp  ].Position.Y=y0;
		vertices[vp  ].Color=color;
		vertices[vp+1].Position.X=x1;vertices[vp+1].Position.Y=y1;
		vertices[vp+1].Color=color;
		
		return 0;
	}

	public virtual int DrawOval( float x,float y,float w,float h ){
		Flush();
		primType=5;
		primTex=null;
		
		float xr=w/2.0f;
		float yr=h/2.0f;

		int segs;
		if( tformed ){
			float dx_x=xr * ix;
			float dx_y=xr * iy;
			float dx=(float)Math.Sqrt( dx_x*dx_x+dx_y*dx_y );
			float dy_x=yr * jx;
			float dy_y=yr * jy;
			float dy=(float)Math.Sqrt( dy_x*dy_x+dy_y*dy_y );
			segs=(int)( dx+dy );
		}else{
			segs=(int)( Math.Abs( xr )+Math.Abs( yr ) );
		}
		segs=Math.Max( segs,12 ) & ~3;
		segs=Math.Min( segs,MAX_VERTS );

		float x0=x+xr,y0=y+yr;

		for( int i=0;i<segs;++i ){
		
			float th=-(float)i * (float)(Math.PI*2.0) / (float)segs;

			float px=x0+(float)Math.Cos( th ) * xr;
			float py=y0-(float)Math.Sin( th ) * yr;
			
			if( tformed ){
				float ppx=px;
				px=ppx * ix + py * jx + tx;
				py=ppx * iy + py * jy + ty;
			}
			
			vertices[i].Position.X=px;vertices[i].Position.Y=py;
			vertices[i].Color=color;
		}
		
		primCount=segs;

		Flush();
		
		return 0;
	}
	
	public virtual int DrawPoly( float[] verts ){
		int n=verts.Length/2;
		if( n<3 || n>MAX_VERTS ) return 0;
		
		Flush();
		primType=5;
		primTex=null;
		
		for( int i=0;i<n;++i ){
		
			float px=verts[i*2];
			float py=verts[i*2+1];
			
			if( tformed ){
				float ppx=px;
				px=ppx * ix + py * jx + tx;
				py=ppx * iy + py * jy + ty;
			}
			
			vertices[i].Position.X=px;vertices[i].Position.Y=py;
			vertices[i].Color=color;
		}

		primCount=n;
		
		Flush();
		
		return 0;
	}

	public virtual int DrawSurface( gxtkSurface surf,float x,float y ){
		if( primType!=4 || primCount==MAX_QUADS || surf.texture!=primTex ){
			Flush();
			primType=4;
			primTex=surf.texture;
		}
		
		float w=surf.Width();
		float h=surf.Height();
		float u0=0,u1=1,v0=0,v1=1;
		float x0=x,x1=x+w,x2=x+w,x3=x;
		float y0=y,y1=y,y2=y+h,y3=y+h;
		
		if( tformed ){
			float tx0=x0,tx1=x1,tx2=x2,tx3=x3;
			x0=tx0 * ix + y0 * jx + tx;
			y0=tx0 * iy + y0 * jy + ty;
			x1=tx1 * ix + y1 * jx + tx;
			y1=tx1 * iy + y1 * jy + ty;
			x2=tx2 * ix + y2 * jx + tx;
			y2=tx2 * iy + y2 * jy + ty;
			x3=tx3 * ix + y3 * jx + tx;
			y3=tx3 * iy + y3 * jy + ty;
		}

		int vp=primCount++*4;
				
		vertices[vp  ].Position.X=x0;vertices[vp  ].Position.Y=y0;
		vertices[vp  ].TextureCoordinate.X=u0;vertices[vp  ].TextureCoordinate.Y=v0;
		vertices[vp  ].Color=color;
		vertices[vp+1].Position.X=x1;vertices[vp+1].Position.Y=y1;
		vertices[vp+1].TextureCoordinate.X=u1;vertices[vp+1].TextureCoordinate.Y=v0;
		vertices[vp+1 ].Color=color;
		vertices[vp+2].Position.X=x2;vertices[vp+2].Position.Y=y2;
		vertices[vp+2].TextureCoordinate.X=u1;vertices[vp+2].TextureCoordinate.Y=v1;
		vertices[vp+2].Color=color;
		vertices[vp+3].Position.X=x3;vertices[vp+3].Position.Y=y3;
		vertices[vp+3].TextureCoordinate.X=u0;vertices[vp+3].TextureCoordinate.Y=v1;
		vertices[vp+3].Color=color;
		
		return 0;
	}

	public virtual int DrawSurface2( gxtkSurface surf,float x,float y,int srcx,int srcy,int srcw,int srch ){
		if( primType!=4 || primCount==MAX_QUADS || surf.texture!=primTex ){
			Flush();
			primType=4;
			primTex=surf.texture;
		}
		
		float w=surf.Width();
		float h=surf.Height();
		float u0=srcx/w,u1=(srcx+srcw)/w;
		float v0=srcy/h,v1=(srcy+srch)/h;
		float x0=x,x1=x+srcw,x2=x+srcw,x3=x;
		float y0=y,y1=y,y2=y+srch,y3=y+srch;
		
		if( tformed ){
			float tx0=x0,tx1=x1,tx2=x2,tx3=x3;
			x0=tx0 * ix + y0 * jx + tx;
			y0=tx0 * iy + y0 * jy + ty;
			x1=tx1 * ix + y1 * jx + tx;
			y1=tx1 * iy + y1 * jy + ty;
			x2=tx2 * ix + y2 * jx + tx;
			y2=tx2 * iy + y2 * jy + ty;
			x3=tx3 * ix + y3 * jx + tx;
			y3=tx3 * iy + y3 * jy + ty;
		}

		int vp=primCount++*4;
				
		vertices[vp  ].Position.X=x0;vertices[vp  ].Position.Y=y0;
		vertices[vp  ].TextureCoordinate.X=u0;vertices[vp  ].TextureCoordinate.Y=v0;
		vertices[vp  ].Color=color;
		vertices[vp+1].Position.X=x1;vertices[vp+1].Position.Y=y1;
		vertices[vp+1].TextureCoordinate.X=u1;vertices[vp+1].TextureCoordinate.Y=v0;
		vertices[vp+1 ].Color=color;
		vertices[vp+2].Position.X=x2;vertices[vp+2].Position.Y=y2;
		vertices[vp+2].TextureCoordinate.X=u1;vertices[vp+2].TextureCoordinate.Y=v1;
		vertices[vp+2].Color=color;
		vertices[vp+3].Position.X=x3;vertices[vp+3].Position.Y=y3;
		vertices[vp+3].TextureCoordinate.X=u0;vertices[vp+3].TextureCoordinate.Y=v1;
		vertices[vp+3].Color=color;
		
		return 0;
	}
	
	public virtual int ReadPixels( int[] pixels,int x,int y,int width,int height,int offset,int pitch ){

		Flush();
		
		Color[] data=new Color[width*height];

		device.SetRenderTarget( null );
		
		renderTarget.GetData( 0,new Rectangle( x,y,width,height ),data,0,data.Length );
		
		device.SetRenderTarget( renderTarget );
		
		int i=0;
		for( int py=0;py<height;++py ){
			int j=offset+py*pitch;
			for( int px=0;px<width;++px ){
				Color c=data[i++];
				pixels[j++]=(c.A<<24) | (c.R<<16) | (c.G<<8) | c.B;
			}
		}
		
		return 0;
	}
	
	public virtual int WritePixels2( gxtkSurface surface,int[] pixels,int x,int y,int width,int height,int offset,int pitch ){
	
		Flush();
	
		Color[] data=new Color[width*height];

		int i=0;
		for( int py=0;py<height;++py ){
			int j=offset+py*pitch;
			for( int px=0;px<width;++px ){
				int argb=pixels[j++];
				data[i++]=new Color( (argb>>16) & 0xff,(argb>>8) & 0xff,argb & 0xff,(argb>>24) & 0xff );
			}
		}
		
		surface.texture.SetData( 0,new Rectangle( x,y,width,height ),data,0,data.Length );
		
		return 0;
	}
}

//***** gxtkSurface *****

public class gxtkSurface{
	public Texture2D texture;
	
	public gxtkSurface(){
	}
	
	public gxtkSurface( Texture2D texture ){
		this.texture=texture;
	}
	
	public void SetTexture( Texture2D texture ){
		this.texture=texture;
	}
	
	//***** GXTK API *****
	
	public virtual int Discard(){
		texture=null;
		return 0;
	}
	
	public virtual int Width(){
		return texture.Width;
	}
	
	public virtual int Height(){
		return texture.Height;
	}
	
	public virtual int Loaded(){
		return 1;
	}
	
	public virtual bool OnUnsafeLoadComplete(){
		return true;
	}
}

public class gxtkInput{

	public bool shift,control;
	
	public int[] keyStates=new int[512];
	
	public int charPut=0;
	public int charGet=0;
	public int[] charQueue=new int[32];
	
	public float mouseX;
	public float mouseY;
	
	public GamePadState gamepadState;
	
	public int[] touches=new int[32];
	public float[] touchX=new float[32];
	public float[] touchY=new float[32];
	
	public float accelX;
	public float accelY;
	public float accelZ;
	
#if WINDOWS_PHONE
	public Accelerometer accelerometer;
	public bool keyboardEnabled=true;
	public bool gamepadEnabled=true;	//for back button mainly!
	public bool mouseEnabled=false;
	public bool touchEnabled=true;
	public bool gamepadFound=true;
	public PlayerIndex gamepadIndex=PlayerIndex.One;
#else
	public bool keyboardEnabled=true;
	public bool gamepadEnabled=true;
	public bool mouseEnabled=true;
	public bool touchEnabled=false;
	public bool gamepadFound=false;
	public PlayerIndex gamepadIndex;
#endif
	
	public const int KEY_LMB=1;
	public const int KEY_RMB=2;
	public const int KEY_MMB=3;
	
	public const int KEY_ESC=27;
	
	public const int KEY_JOY0_A=0x100;
	public const int KEY_JOY0_B=0x101;
	public const int KEY_JOY0_X=0x102;
	public const int KEY_JOY0_Y=0x103;
	public const int KEY_JOY0_LB=0x104;
	public const int KEY_JOY0_RB=0x105;
	public const int KEY_JOY0_BACK=0x106;
	public const int KEY_JOY0_START=0x107;
	public const int KEY_JOY0_LEFT=0x108;
	public const int KEY_JOY0_UP=0x109;
	public const int KEY_JOY0_RIGHT=0x10a;
	public const int KEY_JOY0_DOWN=0x10b;
	
	public const int KEY_TOUCH0=0x180;
	
	public const int KEY_SHIFT=0x10;
	public const int KEY_CONTROL=0x11;
	
	public const int VKEY_SHIFT=0x10;
	public const int VKEY_CONTROL=0x11;
	
	public const int VKEY_LSHIFT=0xa0;
	public const int VKEY_RSHIFT=0xa1;
	public const int VKEY_LCONTROL=0xa2;
	public const int VKEY_RCONTROL=0xa3;
	
#if WINDOWS_PHONE
	public gxtkInput(){
		if( MonkeyConfig.XNA_ACCELEROMETER_ENABLED=="1" ){
			accelerometer=new Accelerometer();
			if( accelerometer.State!=SensorState.NotSupported ){
				accelerometer.ReadingChanged+=OnAccelerometerReadingChanged;
				accelerometer.Start();
			}
        }
	}

	private void OnAccelerometerReadingChanged( object sender,AccelerometerReadingEventArgs e ){
		accelX=(float)e.X;
		accelY=(float)e.Y;
		accelZ=(float)e.Z;
    }		
#endif
	
	public int KeyToChar( int key ){
		if( key==8 || key==9 || key==13 || key==27 || key==32 ){
			return key;
		}else if( key==46 ){
			return 127;
		}else if( key>=48 && key<=57 && !shift ){
			return key;
		}else if( key>=65 && key<=90 && !shift ){
			return key+32;
		}else if( key>=65 && key<=90 && shift ){
			return key;
		}else if( key>=33 && key<=40 || key==45 ){
			return key | 0x10000;
		}
		return 0;
	}
	
	public void BeginUpdate(){

	
		//***** Update keyboard *****
		//
		if( keyboardEnabled ){

			KeyboardState keyboardState=Keyboard.GetState();
			
			shift=keyboardState.IsKeyDown( Keys.LeftShift ) || keyboardState.IsKeyDown( Keys.RightShift );
			control=keyboardState.IsKeyDown( Keys.LeftControl ) || keyboardState.IsKeyDown( Keys.RightControl );
			
			OnKey( KEY_SHIFT,shift );
			OnKey( KEY_CONTROL,control );

			for( int i=8;i<256;++i ){
				if( i==KEY_SHIFT || i==KEY_CONTROL ) continue;
				OnKey( i,keyboardState.IsKeyDown( (Keys)i ) );
			}
		}
		
		//***** Update gamepad *****
		//
		if( gamepadEnabled ){
			if( gamepadFound ){
				gamepadState=GamePad.GetState( gamepadIndex );
				PollGamepadState();
			}else{
				for( PlayerIndex i=PlayerIndex.One;i<=PlayerIndex.Four;++i ){
					GamePadState g=GamePad.GetState( i );
					if( !g.IsConnected ) continue;
					ButtonState p=ButtonState.Pressed;
					if( 
					g.Buttons.A==p ||
					g.Buttons.B==p ||
					g.Buttons.X==p ||
					g.Buttons.Y==p ||
					g.Buttons.LeftShoulder==p ||
					g.Buttons.RightShoulder==p ||
					g.Buttons.Back==p ||
					g.Buttons.Start==p ||
					g.DPad.Left==p ||
					g.DPad.Up==p ||
					g.DPad.Right==p ||
					g.DPad.Down==p ){
						gamepadFound=true;
						gamepadIndex=i;
						gamepadState=g;
						PollGamepadState();
						break;
					}
				}
			}
		}

		//***** Update mouse *****
		//
		if( mouseEnabled ){

			MouseState mouseState=Mouse.GetState();
			
			OnKey( KEY_LMB,mouseState.LeftButton==ButtonState.Pressed );
			OnKey( KEY_RMB,mouseState.RightButton==ButtonState.Pressed );
			OnKey( KEY_MMB,mouseState.MiddleButton==ButtonState.Pressed );
			
			mouseX=mouseState.X;
			mouseY=mouseState.Y;
			if( !touchEnabled ){
				touchX[0]=mouseX;
				touchY[0]=mouseY;
			}
		}
		
		//***** Update touch *****
		//
		if( touchEnabled ){
#if WINDOWS_PHONE
			TouchCollection touchCollection=TouchPanel.GetState();
			foreach( TouchLocation tl in touchCollection ){
			
				if( tl.State==TouchLocationState.Invalid ) continue;
			
				int touch=tl.Id;
				
				int pid;
				for( pid=0;pid<32 && touches[pid]!=touch;++pid ){}
	
				switch( tl.State ){
				case TouchLocationState.Pressed:
					if( pid!=32 ){ pid=32;break; }
					for( pid=0;pid<32 && touches[pid]!=0;++pid ){}
					if( pid==32 ) break;
					touches[pid]=touch;
					OnKeyDown( KEY_TOUCH0+pid );
//					keyStates[KEY_TOUCH0+pid]=0x101;
					break;
				case TouchLocationState.Moved:
					break;
				case TouchLocationState.Released:
					if( pid==32 ) break;
					touches[pid]=0;
					OnKeyUp( KEY_TOUCH0+pid );
//					keyStates[KEY_TOUCH0+pid]=0;
					break;
				}
				if( pid==32 ){
					//ERROR!
					continue;
				}
				Vector2 p=tl.Position;
				touchX[pid]=p.X;
				touchY[pid]=p.Y;
				if( pid==0 && !mouseEnabled ){
					mouseX=p.X;
					mouseY=p.Y;
				}
			}
#endif			
		}
	}
	
	public void PollGamepadState(){
		OnKey( KEY_JOY0_A,gamepadState.Buttons.A==ButtonState.Pressed );
		OnKey( KEY_JOY0_B,gamepadState.Buttons.B==ButtonState.Pressed );
		OnKey( KEY_JOY0_X,gamepadState.Buttons.X==ButtonState.Pressed );
		OnKey( KEY_JOY0_Y,gamepadState.Buttons.Y==ButtonState.Pressed );
		OnKey( KEY_JOY0_LB,gamepadState.Buttons.LeftShoulder==ButtonState.Pressed );
		OnKey( KEY_JOY0_RB,gamepadState.Buttons.RightShoulder==ButtonState.Pressed );
		OnKey( KEY_JOY0_BACK,gamepadState.Buttons.Back==ButtonState.Pressed );
		OnKey( KEY_JOY0_START,gamepadState.Buttons.Start==ButtonState.Pressed );
		OnKey( KEY_JOY0_LEFT,gamepadState.DPad.Left==ButtonState.Pressed );
		OnKey( KEY_JOY0_UP,gamepadState.DPad.Up==ButtonState.Pressed );
		OnKey( KEY_JOY0_RIGHT,gamepadState.DPad.Right==ButtonState.Pressed );
		OnKey( KEY_JOY0_DOWN,gamepadState.DPad.Down==ButtonState.Pressed );
	}
	
	public void EndUpdate(){
		for( int i=0;i<512;++i ){
			keyStates[i]&=0x100;
		}
		charGet=0;
		charPut=0;
	}
	
	public virtual void OnKey( int key,bool down ){
		if( down ){
			OnKeyDown( key );
		}else{
			OnKeyUp( key );
		}
	}
	
	public virtual void OnKeyDown( int key ){
		if( (keyStates[key] & 0x100)!=0 ) return;
		
		keyStates[key]|=0x100;
		++keyStates[key];
		
		int chr=KeyToChar( key );
		if( chr!=0 ) PutChar( chr );

		if( key==KEY_LMB && !touchEnabled ){
			this.keyStates[KEY_TOUCH0]|=0x100;
			++this.keyStates[KEY_TOUCH0];
		}else if( key==KEY_TOUCH0 && !mouseEnabled ){
			this.keyStates[KEY_LMB]|=0x100;
			++this.keyStates[KEY_LMB];
		}
	}
	
	public virtual void OnKeyUp( int key ){
		if( (keyStates[key] & 0x100)==0 ) return;
		
		keyStates[key]&=0xff;

		if( key==KEY_LMB && !touchEnabled ){
			this.keyStates[KEY_TOUCH0]&=0xff;
		}else if( key==KEY_TOUCH0 && !mouseEnabled ){
			this.keyStates[KEY_LMB]&=0xff;
		}
	}
	
	public virtual void PutChar( int chr ){
		if( charPut!=32 ){
			charQueue[charPut++]=chr;
		}
	}

	//***** GXTK API *****
	
	public virtual int SetKeyboardEnabled( int enabled ){
#if WINDOWS
		return 0;	//keyboard present on windows
#else
		return -1;	//no keyboard support on XBOX/PHONE
#endif
	}
	
	public virtual int KeyDown( int key ){
		if( key>0 && key<512 ){
			return keyStates[key]>>8;
		}
		return 0;
	}
	
	public virtual int KeyHit( int key ){
		if( key>0 && key<512 ){
			return keyStates[key] & 0xff;
		}
		return 0;
	}
	
	public virtual int GetChar(){
		if( charGet!=charPut ){
			return charQueue[charGet++];
		}
		return 0;
	}
	
	public virtual float MouseX(){
		return mouseX;
	}
	
	public virtual float MouseY(){
		return mouseY;
	}

	public virtual float JoyX( int index ){
		switch( index ){
		case 0:return gamepadState.ThumbSticks.Left.X;
		case 1:return gamepadState.ThumbSticks.Right.X;
		}
		return 0;
	}
	
	public virtual float JoyY( int index ){
		switch( index ){
		case 0:return gamepadState.ThumbSticks.Left.Y;
		case 1:return gamepadState.ThumbSticks.Right.Y;
		}
		return 0;
	}
	
	public virtual float JoyZ( int index ){
		switch( index ){
		case 0:return gamepadState.Triggers.Left;
		case 1:return gamepadState.Triggers.Right;
		}
		return 0;
	}
	
	public virtual float TouchX( int index ){
		return touchX[index];
	}

	public virtual float TouchY( int index ){
		return touchY[index];
	}
	
	public virtual float AccelX(){
		return accelX;
	}

	public virtual float AccelY(){
		return accelY;
	}

	public virtual float AccelZ(){
		return accelZ;
	}
}

public class gxtkChannel{
	public gxtkSample sample;
	public SoundEffectInstance inst;
	public float volume=1;
	public float pan=0;
	public float rate=1;
	public int state=0;
};

public class gxtkAudio{

	public bool musicEnabled;

	public gxtkChannel[] channels=new gxtkChannel[33];
	
	public void OnSuspend(){
		for( int i=0;i<33;++i ){
			if( channels[i].state==1 ) channels[i].inst.Pause();
		}
	}
	
	public void OnResume(){
		for( int i=0;i<33;++i ){
			if( channels[i].state==1 ) channels[i].inst.Resume();
		}
	}

	//***** GXTK API *****
	//
	public gxtkAudio(){
		musicEnabled=MediaPlayer.GameHasControl;
		
		for( int i=0;i<33;++i ){
			channels[i]=new gxtkChannel();
		}
	}
	
	public virtual gxtkSample LoadSample__UNSAFE__( gxtkSample sample,String path ){
		SoundEffect sound=MonkeyData.LoadSoundEffect( path,gxtkApp.game.Content );
		if( sound==null ) return null;
		sample.SetSound( sound );
		return sample;
	}
	
	public virtual gxtkSample LoadSample( String path ){
		return LoadSample__UNSAFE__( new gxtkSample(),path );
	}
	
	public virtual int PlaySample( gxtkSample sample,int channel,int flags ){
		gxtkChannel chan=channels[channel];

		SoundEffectInstance inst=null;
		
		if( chan.state!=0 ){
			chan.inst.Stop();
			chan.state=0;
		}
		inst=sample.AllocInstance( (flags&1)!=0 );
		if( inst==null ) return -1;
		
		for( int i=0;i<33;++i ){
			gxtkChannel chan2=channels[i];
			if( chan2.inst==inst ){
				chan2.sample=null;
				chan2.inst=null;
				chan2.state=0;
				break;
			}
		}
		
		inst.Volume=chan.volume;
		inst.Pan=chan.pan;
		inst.Pitch=(float)( Math.Log(chan.rate)/Math.Log(2) );
		inst.Play();

		chan.sample=sample;
		chan.inst=inst;
		chan.state=1;
		return 0;
	}
	
	public virtual int StopChannel( int channel ){
		gxtkChannel chan=channels[channel];
		
		if( chan.state!=0 ){
			chan.inst.Stop();
			chan.state=0;
		}
		return 0;
	}
	
	public virtual int PauseChannel( int channel ){
		gxtkChannel chan=channels[channel];
		
		if( chan.state==1 ){
			chan.inst.Pause();
			chan.state=2;
		}
		return 0;
	}
	
	public virtual int ResumeChannel( int channel ){
		gxtkChannel chan=channels[channel];
		
		if( chan.state==2 ){
			chan.inst.Resume();
			chan.state=1;
		}
		return 0;
	}
	
	public virtual int ChannelState( int channel ){
		gxtkChannel chan=channels[channel];
		
		if( chan.state==1 ){
			if( chan.inst.State!=SoundState.Playing ) chan.state=0;
		}
		
		return chan.state;
	}
	
	public virtual int SetVolume( int channel,float volume ){
		gxtkChannel chan=channels[channel];
		
		if( chan.state!=0 ) chan.inst.Volume=volume;
		
		chan.volume=volume;
		return 0;
	}
	
	public virtual int SetPan( int channel,float pan ){
		gxtkChannel chan=channels[channel];
		
		if( chan.state!=0 ) chan.inst.Pan=pan;
		
		chan.pan=pan;
		return 0;
	}
	
	public virtual int SetRate( int channel,float rate ){
		gxtkChannel chan=channels[channel];
		
		if( chan.state!=0 ) chan.inst.Pitch=(float)( Math.Log(rate)/Math.Log(2) );
		
		chan.rate=rate;
		return 0;
	}
	
	public virtual int PlayMusic( String path,int flags ){
		if( !musicEnabled ) return -1;
		
		MediaPlayer.Stop();
		
		Song song=MonkeyData.LoadSong( path,gxtkApp.game.Content );
		if( song==null ) return -1;
		
		if( (flags&1)!=0 ) MediaPlayer.IsRepeating=true;
		
		MediaPlayer.Play( song );
		return 0;
	}
	
	public virtual int StopMusic(){
		if( !musicEnabled ) return -1;
		
		MediaPlayer.Stop();
		return 0;
	}
	
	public virtual int PauseMusic(){
		if( !musicEnabled ) return -1;
		
		MediaPlayer.Pause();
		return 0;
	}
	
	public virtual int ResumeMusic(){
		if( !musicEnabled ) return -1;
		
		MediaPlayer.Resume();
		return 0;
	}
	
	public virtual int MusicState(){
		if( !musicEnabled ) return -1;
		
		return MediaPlayer.State==MediaState.Playing ? 1 : 0;
	}
	
	public virtual int SetMusicVolume( float volume ){
		if( !musicEnabled ) return -1;
		
		MediaPlayer.Volume=volume;
		return 0;
	}
}

public class gxtkSample{

	public SoundEffect sound;
	
	//first 8 non-looped, second 8 looped.
	public SoundEffectInstance[] insts=new SoundEffectInstance[16];	
	
	public gxtkSample(){
	}
	
	public gxtkSample( SoundEffect sound ){
		this.sound=sound;
	}
	
	public void SetSound( SoundEffect sound ){
		this.sound=sound;
	}

	public SoundEffectInstance AllocInstance( bool looped ){
		int st=looped ? 8 : 0;
		for( int i=st;i<st+8;++i ){
			SoundEffectInstance inst=insts[i];
			if( inst!=null ){
				if( inst.State!=SoundState.Playing ) return inst;
			}else{
				inst=sound.CreateInstance();
				inst.IsLooped=looped;
				insts[i]=inst;
				return inst;
			}
		}
		return null;
	}

	public virtual int Discard(){	
		if( sound!=null ){
			sound=null;
			for( int i=0;i<16;++i ){
				insts[i]=null;
			}
		}
		return 0;
	}	
}

class BBThread{

	private bool _running;
	private Thread _thread;
	
	public virtual void Start(){
		if( _running ) return;
		_running=true;
		_thread=new Thread( new ThreadStart( this.run ) );
		_thread.Start();
	}
	
	public virtual bool IsRunning(){
		return _running;
	}

	public virtual void Run__UNSAFE__(){
	}

	private void run(){
		Run__UNSAFE__();
		_running=false;
	}
}
class bb_app_App : Object{
	public virtual bb_app_App g_App_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<109>";
		bb_app.bb_app_device=(new bb_app_AppDevice()).g_AppDevice_new(this);
		bb_std_lang.popErr();
		return this;
	}
	public virtual int m_OnCreate(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_OnUpdate(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_OnSuspend(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_OnResume(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_OnRender(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_OnLoading(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return 0;
	}
}
class bb__LDApp : bb_app_App{
	public virtual bb__LDApp g_LDApp_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<46>";
		base.g_App_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<46>";
		bb_std_lang.popErr();
		return this;
	}
	public static int g_ScreenHeight;
	public static int g_ScreenWidth;
	public static int g_RefreshRate;
	public static float g_Delta;
	public override int m_OnCreate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<81>";
		bb_gfx_GFX.g_Init();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<82>";
		bb_screenmanager_ScreenManager.g_Init();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<83>";
		bb_sfx_SFX.g_Init();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<84>";
		bb_controls_Controls.g_Init();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<87>";
		bb_raztext_RazText.g_SetTextSheet(bb_graphics.bb_graphics_LoadImage("gfx/fonts.png",1,bb_graphics_Image.g_DefaultFlags));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<93>";
		bb_screenmanager_ScreenManager.g_AddScreen("game",((new bb_gamescreen_GameScreen()).g_GameScreen_new()));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<101>";
		bb_sfx_SFX.g_AddMusic("ambient","ambient.mp3");
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<102>";
		bb_sfx_SFX.g_AddMusic("chase","chase.mp3");
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<105>";
		bb_screenmanager_ScreenManager.g_SetFadeColour(0.0f,0.0f,0.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<106>";
		bb_screenmanager_ScreenManager.g_SetFadeRate(0.1f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<107>";
		bb_screenmanager_ScreenManager.g_SetScreen("game");
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<109>";
		if(bb_controls_Controls.g_ControlMethod==2){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<110>";
			g_RefreshRate=30;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<112>";
			g_RefreshRate=60;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<115>";
		bb_app.bb_app_SetUpdateRate(g_RefreshRate);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<116>";
		g_Delta=(float)(g_RefreshRate)/60.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<119>";
		bb_autofit.bb_autofit_SetVirtualDisplay(g_ScreenWidth,g_ScreenHeight,1.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<121>";
		bb_std_lang.popErr();
		return 0;
	}
	public static float g_TargetScreenX;
	public static int g_ActualScreenX;
	public static float g_ScreenMoveRate;
	public static float g_TargetScreenY;
	public static int g_ActualScreenY;
	public static int g_ScreenX;
	public static int g_ScreenY;
	public override int m_OnUpdate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<126>";
		bb_controls_Controls.g_Update();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<128>";
		g_ActualScreenX=(int)((float)(g_ActualScreenX)+(g_TargetScreenX-(float)(g_ActualScreenX))*g_ScreenMoveRate);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<129>";
		g_ActualScreenY=(int)((float)(g_ActualScreenY)+(g_TargetScreenY-(float)(g_ActualScreenY))*g_ScreenMoveRate);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<131>";
		g_ScreenX=g_ActualScreenX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<132>";
		g_ScreenY=g_ActualScreenY;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<134>";
		bb_screenmanager_ScreenManager.g_Update();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<135>";
		bb_std_lang.popErr();
		return 0;
	}
	public override int m_OnRender(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<140>";
		bb_autofit.bb_autofit_UpdateVirtualDisplay(true,true);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<142>";
		bb_graphics.bb_graphics_Cls(0.0f,0.0f,0.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<143>";
		bb_screenmanager_ScreenManager.g_Render();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<144>";
		bb_std_lang.popErr();
		return 0;
	}
	public static bb_level_Level g_level;
	public static void g_SetScreenTarget(float t_tX,float t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<67>";
		g_TargetScreenX=t_tX-(float)(g_ScreenWidth)*0.5f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<68>";
		g_TargetScreenY=t_tY-(float)(g_ScreenHeight)*0.5f;
		bb_std_lang.popErr();
	}
	public static void g_SetScreenPosition(float t_tX,float t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<73>";
		g_SetScreenTarget(t_tX,t_tY);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<74>";
		g_ActualScreenX=(int)(g_TargetScreenX);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<75>";
		g_ActualScreenY=(int)(g_TargetScreenY);
		bb_std_lang.popErr();
	}
}
class bb_app_AppDevice : gxtkApp{
	public bb_app_App f_app=null;
	public virtual bb_app_AppDevice g_AppDevice_new(bb_app_App t_app){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<49>";
		this.f_app=t_app;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<50>";
		bb_graphics.bb_graphics_SetGraphicsDevice(GraphicsDevice());
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<51>";
		bb_input.bb_input_SetInputDevice(InputDevice());
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<52>";
		bb_audio.bb_audio_SetAudioDevice(AudioDevice());
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_app_AppDevice g_AppDevice_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<46>";
		bb_std_lang.popErr();
		return this;
	}
	public override int OnCreate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<56>";
		bb_graphics.bb_graphics_SetFont(null,32);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<57>";
		int t_=f_app.m_OnCreate();
		bb_std_lang.popErr();
		return t_;
	}
	public override int OnUpdate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<61>";
		int t_=f_app.m_OnUpdate();
		bb_std_lang.popErr();
		return t_;
	}
	public override int OnSuspend(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<65>";
		int t_=f_app.m_OnSuspend();
		bb_std_lang.popErr();
		return t_;
	}
	public override int OnResume(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<69>";
		int t_=f_app.m_OnResume();
		bb_std_lang.popErr();
		return t_;
	}
	public override int OnRender(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<73>";
		bb_graphics.bb_graphics_BeginRender();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<74>";
		int t_r=f_app.m_OnRender();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<75>";
		bb_graphics.bb_graphics_EndRender();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<76>";
		bb_std_lang.popErr();
		return t_r;
	}
	public override int OnLoading(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<80>";
		bb_graphics.bb_graphics_BeginRender();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<81>";
		int t_r=f_app.m_OnLoading();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<82>";
		bb_graphics.bb_graphics_EndRender();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<83>";
		bb_std_lang.popErr();
		return t_r;
	}
	public int f_updateRate=0;
	public override int SetUpdateRate(int t_hertz){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<87>";
		base.SetUpdateRate(t_hertz);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<88>";
		f_updateRate=t_hertz;
		bb_std_lang.popErr();
		return 0;
	}
}
class bb_graphics_Image : Object{
	public static int g_DefaultFlags;
	public virtual bb_graphics_Image g_Image_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<65>";
		bb_std_lang.popErr();
		return this;
	}
	public gxtkSurface f_surface=null;
	public int f_width=0;
	public int f_height=0;
	public bb_graphics_Frame[] f_frames=new bb_graphics_Frame[0];
	public int f_flags=0;
	public float f_tx=.0f;
	public float f_ty=.0f;
	public virtual int m_SetHandle(float t_tx,float t_ty){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<109>";
		this.f_tx=t_tx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<110>";
		this.f_ty=t_ty;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<111>";
		this.f_flags=this.f_flags&-2;
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_ApplyFlags(int t_iflags){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<178>";
		f_flags=t_iflags;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<180>";
		if((f_flags&2)!=0){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<181>";
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<181>";
			bb_graphics_Frame[] t_=f_frames;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<181>";
			int t_2=0;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<181>";
			while(t_2<bb_std_lang.length(t_)){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<181>";
				bb_graphics_Frame t_f=t_[t_2];
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<181>";
				t_2=t_2+1;
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<182>";
				t_f.f_x+=1;
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<184>";
			f_width-=2;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<187>";
		if((f_flags&4)!=0){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<188>";
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<188>";
			bb_graphics_Frame[] t_3=f_frames;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<188>";
			int t_4=0;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<188>";
			while(t_4<bb_std_lang.length(t_3)){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<188>";
				bb_graphics_Frame t_f2=t_3[t_4];
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<188>";
				t_4=t_4+1;
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<189>";
				t_f2.f_y+=1;
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<191>";
			f_height-=2;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<194>";
		if((f_flags&1)!=0){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<195>";
			m_SetHandle((float)(f_width)/2.0f,(float)(f_height)/2.0f);
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<198>";
		if(bb_std_lang.length(f_frames)==1 && f_frames[0].f_x==0 && f_frames[0].f_y==0 && f_width==f_surface.Width() && f_height==f_surface.Height()){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<199>";
			f_flags|=65536;
		}
		bb_std_lang.popErr();
		return 0;
	}
	public virtual bb_graphics_Image m_Init(gxtkSurface t_surf,int t_nframes,int t_iflags){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<136>";
		f_surface=t_surf;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<138>";
		f_width=f_surface.Width()/t_nframes;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<139>";
		f_height=f_surface.Height();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<141>";
		f_frames=new bb_graphics_Frame[t_nframes];
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<142>";
		for(int t_i=0;t_i<t_nframes;t_i=t_i+1){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<143>";
			f_frames[t_i]=(new bb_graphics_Frame()).g_Frame_new(t_i*f_width,0);
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<146>";
		m_ApplyFlags(t_iflags);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<147>";
		bb_std_lang.popErr();
		return this;
	}
	public bb_graphics_Image f_source=null;
	public virtual bb_graphics_Image m_Grab(int t_x,int t_y,int t_iwidth,int t_iheight,int t_nframes,int t_iflags,bb_graphics_Image t_source){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<151>";
		this.f_source=t_source;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<152>";
		f_surface=t_source.f_surface;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<154>";
		f_width=t_iwidth;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<155>";
		f_height=t_iheight;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<157>";
		f_frames=new bb_graphics_Frame[t_nframes];
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<159>";
		int t_ix=t_x;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<159>";
		int t_iy=t_y;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<161>";
		for(int t_i=0;t_i<t_nframes;t_i=t_i+1){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<162>";
			if(t_ix+f_width>t_source.f_width){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<163>";
				t_ix=0;
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<164>";
				t_iy+=f_height;
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<166>";
			if(t_ix+f_width>t_source.f_width || t_iy+f_height>t_source.f_height){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<167>";
				bb_std_lang.Error("Image frame outside surface");
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<169>";
			f_frames[t_i]=(new bb_graphics_Frame()).g_Frame_new(t_ix+t_source.f_frames[0].f_x,t_iy+t_source.f_frames[0].f_y);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<170>";
			t_ix+=f_width;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<173>";
		m_ApplyFlags(t_iflags);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<174>";
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_graphics_Image m_GrabImage(int t_x,int t_y,int t_width,int t_height,int t_frames,int t_flags){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<104>";
		if(bb_std_lang.length(this.f_frames)!=1){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<104>";
			bb_std_lang.popErr();
			return null;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<105>";
		bb_graphics_Image t_=((new bb_graphics_Image()).g_Image_new()).m_Grab(t_x,t_y,t_width,t_height,t_frames,t_flags,this);
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_graphics_GraphicsContext : Object{
	public virtual bb_graphics_GraphicsContext g_GraphicsContext_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<24>";
		bb_std_lang.popErr();
		return this;
	}
	public bb_graphics_Image f_defaultFont=null;
	public bb_graphics_Image f_font=null;
	public int f_firstChar=0;
	public int f_matrixSp=0;
	public float f_ix=1.0f;
	public float f_iy=.0f;
	public float f_jx=.0f;
	public float f_jy=1.0f;
	public float f_tx=.0f;
	public float f_ty=.0f;
	public int f_tformed=0;
	public int f_matDirty=0;
	public float f_color_r=.0f;
	public float f_color_g=.0f;
	public float f_color_b=.0f;
	public float f_alpha=.0f;
	public int f_blend=0;
	public float f_scissor_x=.0f;
	public float f_scissor_y=.0f;
	public float f_scissor_width=.0f;
	public float f_scissor_height=.0f;
	public virtual int m_Validate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<35>";
		if((f_matDirty)!=0){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<36>";
			bb_graphics.bb_graphics_renderDevice.SetMatrix(bb_graphics.bb_graphics_context.f_ix,bb_graphics.bb_graphics_context.f_iy,bb_graphics.bb_graphics_context.f_jx,bb_graphics.bb_graphics_context.f_jy,bb_graphics.bb_graphics_context.f_tx,bb_graphics.bb_graphics_context.f_ty);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<37>";
			f_matDirty=0;
		}
		bb_std_lang.popErr();
		return 0;
	}
	public float[] f_matrixStack=new float[192];
}
class bb_graphics_Frame : Object{
	public int f_x=0;
	public int f_y=0;
	public virtual bb_graphics_Frame g_Frame_new(int t_x,int t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<18>";
		this.f_x=t_x;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<19>";
		this.f_y=t_y;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_graphics_Frame g_Frame_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<13>";
		bb_std_lang.popErr();
		return this;
	}
}
class bb_gfx_GFX : Object{
	public static bb_graphics_Image g_Tileset;
	public static void g_Init(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<8>";
		g_Tileset=bb_graphics.bb_graphics_LoadImage("gfx/sheet.png",1,bb_graphics_Image.g_DefaultFlags);
		bb_std_lang.popErr();
	}
	public static void g_Draw(bb_graphics_Image t_tImage,float t_tX,float t_tY,int t_tF,bool t_Follow){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<20>";
		if(t_Follow==true){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<21>";
			bb_graphics.bb_graphics_DrawImage(t_tImage,t_tX-(float)(bb__LDApp.g_ScreenX),t_tY-(float)(bb__LDApp.g_ScreenY),t_tF);
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<23>";
			bb_graphics.bb_graphics_DrawImage(t_tImage,t_tX,t_tY,t_tF);
		}
		bb_std_lang.popErr();
	}
}
class bb_screenmanager_ScreenManager : Object{
	public static bb_map_StringMap g_Screens;
	public static float g_FadeAlpha;
	public static float g_FadeRate;
	public static float g_FadeRed;
	public static float g_FadeGreen;
	public static float g_FadeBlue;
	public static int g_FadeMode;
	public static void g_Init(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<25>";
		g_Screens=(new bb_map_StringMap()).g_StringMap_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<27>";
		g_FadeAlpha=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<28>";
		g_FadeRate=0.01f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<29>";
		g_FadeRed=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<30>";
		g_FadeGreen=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<31>";
		g_FadeBlue=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<33>";
		g_FadeMode=0;
		bb_std_lang.popErr();
	}
	public static bb_gamescreen_GameScreen g_gameScreen;
	public static void g_AddScreen(String t_tName,bb_screen_Screen t_tScreen){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<97>";
		g_Screens.m_Set(t_tName,t_tScreen);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<98>";
		if(t_tName.CompareTo("game")==0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<99>";
			bb_screen_Screen t_=t_tScreen;
			g_gameScreen=(t_ is bb_gamescreen_GameScreen ? (bb_gamescreen_GameScreen)t_ : null);
		}
		bb_std_lang.popErr();
	}
	public static void g_SetFadeColour(float t_tR,float t_tG,float t_tB){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<128>";
		g_FadeRed=bb_math.bb_math_Clamp2(t_tR,0.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<129>";
		g_FadeGreen=bb_math.bb_math_Clamp2(t_tG,0.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<130>";
		g_FadeBlue=bb_math.bb_math_Clamp2(t_tB,0.0f,255.0f);
		bb_std_lang.popErr();
	}
	public static void g_SetFadeRate(float t_tRate){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<124>";
		g_FadeRate=bb_math.bb_math_Clamp2(t_tRate,0.001f,1.0f);
		bb_std_lang.popErr();
	}
	public static bb_screen_Screen g_ActiveScreen;
	public static String g_ActiveScreenName;
	public static void g_SetScreen(String t_tName){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<105>";
		if(g_ActiveScreen!=null){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<106>";
			g_ActiveScreen.m_OnScreenEnd();
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<108>";
		g_ActiveScreen=g_Screens.m_Get(t_tName);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<109>";
		g_ActiveScreen.m_OnScreenStart();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<110>";
		g_FadeMode=1;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<111>";
		g_ActiveScreenName=t_tName;
		bb_std_lang.popErr();
	}
	public static String g_NextScreenName;
	public static void g_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<39>";
		if(g_ActiveScreen!=null){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<40>";
			g_ActiveScreen.m_Update();
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<43>";
		int t_=g_FadeMode;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<44>";
		if(t_==0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<45>";
			g_FadeAlpha-=g_FadeRate;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<46>";
			if(g_FadeAlpha<=0.0f){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<47>";
				g_FadeAlpha=0.0f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<48>";
				g_FadeMode=1;
			}
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<51>";
			if(t_==1){
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<53>";
				if(t_==2){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<54>";
					g_FadeAlpha+=g_FadeRate;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<55>";
					if(g_FadeAlpha>=1.0f){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<56>";
						g_FadeAlpha=1.0f;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<57>";
						g_FadeMode=0;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<58>";
						if(g_ActiveScreen!=null){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<59>";
							g_ActiveScreen.m_OnScreenEnd();
						}
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<61>";
						g_ActiveScreenName=g_NextScreenName;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<62>";
						g_ActiveScreen=g_Screens.m_Get(g_ActiveScreenName);
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<63>";
						g_ActiveScreen.m_OnScreenStart();
					}
				}
			}
		}
		bb_std_lang.popErr();
	}
	public static void g_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<71>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<72>";
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<74>";
		if(g_ActiveScreen!=null){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<75>";
			g_ActiveScreen.m_Render();
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<78>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<79>";
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<80>";
		if(bb_controls_Controls.g_ControlMethod==2){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<81>";
			bb_controls_Controls.g_TCMove.m_DoRenderRing();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<82>";
			bb_controls_Controls.g_TCMove.m_DoRenderStick();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<84>";
			bb_controls_Controls.g_TCAction1.m_Render();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<85>";
			bb_controls_Controls.g_TCAction2.m_Render();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<86>";
			bb_controls_Controls.g_TCEscapeKey.m_Render();
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<89>";
		if(g_FadeMode!=1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<90>";
			bb_graphics.bb_graphics_SetColor(g_FadeRed,g_FadeGreen,g_FadeBlue);
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<91>";
			bb_graphics.bb_graphics_SetAlpha(g_FadeAlpha);
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/screenmanager.monkey<92>";
			bb_graphics.bb_graphics_DrawRect(0.0f,0.0f,(float)(bb_graphics.bb_graphics_DeviceWidth()),(float)(bb_graphics.bb_graphics_DeviceHeight()));
		}
		bb_std_lang.popErr();
	}
}
class bb_screen_Screen : Object{
	public virtual bb_screen_Screen g_Screen_new(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return this;
	}
	public virtual void m_OnScreenEnd(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_OnScreenStart(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
}
abstract class bb_map_Map : Object{
	public virtual bb_map_Map g_Map_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<7>";
		bb_std_lang.popErr();
		return this;
	}
	public bb_map_Node f_root=null;
	public abstract int m_Compare(String t_lhs,String t_rhs);
	public virtual int m_RotateLeft(bb_map_Node t_node){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<251>";
		bb_map_Node t_child=t_node.f_right;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<252>";
		t_node.f_right=t_child.f_left;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<253>";
		if((t_child.f_left)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<254>";
			t_child.f_left.f_parent=t_node;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<256>";
		t_child.f_parent=t_node.f_parent;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<257>";
		if((t_node.f_parent)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<258>";
			if(t_node==t_node.f_parent.f_left){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<259>";
				t_node.f_parent.f_left=t_child;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<261>";
				t_node.f_parent.f_right=t_child;
			}
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<264>";
			f_root=t_child;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<266>";
		t_child.f_left=t_node;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<267>";
		t_node.f_parent=t_child;
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_RotateRight(bb_map_Node t_node){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<271>";
		bb_map_Node t_child=t_node.f_left;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<272>";
		t_node.f_left=t_child.f_right;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<273>";
		if((t_child.f_right)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<274>";
			t_child.f_right.f_parent=t_node;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<276>";
		t_child.f_parent=t_node.f_parent;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<277>";
		if((t_node.f_parent)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<278>";
			if(t_node==t_node.f_parent.f_right){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<279>";
				t_node.f_parent.f_right=t_child;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<281>";
				t_node.f_parent.f_left=t_child;
			}
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<284>";
			f_root=t_child;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<286>";
		t_child.f_right=t_node;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<287>";
		t_node.f_parent=t_child;
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_InsertFixup(bb_map_Node t_node){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<212>";
		while(((t_node.f_parent)!=null) && t_node.f_parent.f_color==-1 && ((t_node.f_parent.f_parent)!=null)){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<213>";
			if(t_node.f_parent==t_node.f_parent.f_parent.f_left){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<214>";
				bb_map_Node t_uncle=t_node.f_parent.f_parent.f_right;
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<215>";
				if(((t_uncle)!=null) && t_uncle.f_color==-1){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<216>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<217>";
					t_uncle.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<218>";
					t_uncle.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<219>";
					t_node=t_uncle.f_parent;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<221>";
					if(t_node==t_node.f_parent.f_right){
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<222>";
						t_node=t_node.f_parent;
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<223>";
						m_RotateLeft(t_node);
					}
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<225>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<226>";
					t_node.f_parent.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<227>";
					m_RotateRight(t_node.f_parent.f_parent);
				}
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<230>";
				bb_map_Node t_uncle2=t_node.f_parent.f_parent.f_left;
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<231>";
				if(((t_uncle2)!=null) && t_uncle2.f_color==-1){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<232>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<233>";
					t_uncle2.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<234>";
					t_uncle2.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<235>";
					t_node=t_uncle2.f_parent;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<237>";
					if(t_node==t_node.f_parent.f_left){
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<238>";
						t_node=t_node.f_parent;
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<239>";
						m_RotateRight(t_node);
					}
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<241>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<242>";
					t_node.f_parent.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<243>";
					m_RotateLeft(t_node.f_parent.f_parent);
				}
			}
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<247>";
		f_root.f_color=1;
		bb_std_lang.popErr();
		return 0;
	}
	public virtual bool m_Set(String t_key,bb_screen_Screen t_value){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<29>";
		bb_map_Node t_node=f_root;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<30>";
		bb_map_Node t_parent=null;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<30>";
		int t_cmp=0;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<32>";
		while((t_node)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<33>";
			t_parent=t_node;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<34>";
			t_cmp=m_Compare(t_key,t_node.f_key);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<35>";
			if(t_cmp>0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<36>";
				t_node=t_node.f_right;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<37>";
				if(t_cmp<0){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<38>";
					t_node=t_node.f_left;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<40>";
					t_node.f_value=t_value;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<41>";
					bb_std_lang.popErr();
					return false;
				}
			}
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<45>";
		t_node=(new bb_map_Node()).g_Node_new(t_key,t_value,-1,t_parent);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<47>";
		if((t_parent)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<48>";
			if(t_cmp>0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<49>";
				t_parent.f_right=t_node;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<51>";
				t_parent.f_left=t_node;
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<53>";
			m_InsertFixup(t_node);
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<55>";
			f_root=t_node;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<57>";
		bb_std_lang.popErr();
		return true;
	}
	public virtual bb_map_Node m_FindNode(String t_key){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<157>";
		bb_map_Node t_node=f_root;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<159>";
		while((t_node)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<160>";
			int t_cmp=m_Compare(t_key,t_node.f_key);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<161>";
			if(t_cmp>0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<162>";
				t_node=t_node.f_right;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<163>";
				if(t_cmp<0){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<164>";
					t_node=t_node.f_left;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<166>";
					bb_std_lang.popErr();
					return t_node;
				}
			}
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<169>";
		bb_std_lang.popErr();
		return t_node;
	}
	public virtual bb_screen_Screen m_Get(String t_key){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<101>";
		bb_map_Node t_node=m_FindNode(t_key);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<102>";
		if((t_node)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<102>";
			bb_std_lang.popErr();
			return t_node.f_value;
		}
		bb_std_lang.popErr();
		return null;
	}
}
class bb_map_StringMap : bb_map_Map{
	public virtual bb_map_StringMap g_StringMap_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<551>";
		base.g_Map_new();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<551>";
		bb_std_lang.popErr();
		return this;
	}
	public override int m_Compare(String t_lhs,String t_rhs){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<554>";
		int t_=t_lhs.CompareTo(t_rhs);
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_sfx_SFX : Object{
	public static int g_ActiveChannel;
	public static bb_map_StringMap2 g_Sounds;
	public static bb_map_StringMap3 g_Musics;
	public static void g_Init(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<21>";
		g_ActiveChannel=0;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<22>";
		g_Sounds=(new bb_map_StringMap2()).g_StringMap_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<23>";
		g_Musics=(new bb_map_StringMap3()).g_StringMap_new();
		bb_std_lang.popErr();
	}
	public static void g_AddMusic(String t_tName,String t_tFile){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<50>";
		g_Musics.m_Set2(t_tName,t_tFile);
		bb_std_lang.popErr();
	}
	public static bool g_MusicActive;
	public static String g_CurrentMusic;
	public static void g_Music(String t_tMus,int t_tLoop){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<83>";
		if(g_MusicActive==false){
			bb_std_lang.popErr();
			return;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<87>";
		if(!g_Musics.m_Contains(t_tMus)){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<88>";
			bb_std_lang.Error("Music "+t_tMus+" does not appear to exist");
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<91>";
		if((t_tMus.CompareTo(g_CurrentMusic)!=0) || (bb_audio.bb_audio_MusicState()==-1 || bb_audio.bb_audio_MusicState()==0)){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<92>";
			bb_audio.bb_audio_PlayMusic("mus/"+g_Musics.m_Get(t_tMus),t_tLoop);
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/sfx.monkey<93>";
			g_CurrentMusic=t_tMus;
		}
		bb_std_lang.popErr();
	}
}
class bb_audio_Sound : Object{
}
abstract class bb_map_Map2 : Object{
	public virtual bb_map_Map2 g_Map_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<7>";
		bb_std_lang.popErr();
		return this;
	}
}
class bb_map_StringMap2 : bb_map_Map2{
	public virtual bb_map_StringMap2 g_StringMap_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<551>";
		base.g_Map_new();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<551>";
		bb_std_lang.popErr();
		return this;
	}
}
abstract class bb_map_Map3 : Object{
	public virtual bb_map_Map3 g_Map_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<7>";
		bb_std_lang.popErr();
		return this;
	}
	public bb_map_Node2 f_root=null;
	public abstract int m_Compare(String t_lhs,String t_rhs);
	public virtual int m_RotateLeft2(bb_map_Node2 t_node){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<251>";
		bb_map_Node2 t_child=t_node.f_right;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<252>";
		t_node.f_right=t_child.f_left;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<253>";
		if((t_child.f_left)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<254>";
			t_child.f_left.f_parent=t_node;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<256>";
		t_child.f_parent=t_node.f_parent;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<257>";
		if((t_node.f_parent)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<258>";
			if(t_node==t_node.f_parent.f_left){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<259>";
				t_node.f_parent.f_left=t_child;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<261>";
				t_node.f_parent.f_right=t_child;
			}
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<264>";
			f_root=t_child;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<266>";
		t_child.f_left=t_node;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<267>";
		t_node.f_parent=t_child;
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_RotateRight2(bb_map_Node2 t_node){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<271>";
		bb_map_Node2 t_child=t_node.f_left;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<272>";
		t_node.f_left=t_child.f_right;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<273>";
		if((t_child.f_right)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<274>";
			t_child.f_right.f_parent=t_node;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<276>";
		t_child.f_parent=t_node.f_parent;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<277>";
		if((t_node.f_parent)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<278>";
			if(t_node==t_node.f_parent.f_right){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<279>";
				t_node.f_parent.f_right=t_child;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<281>";
				t_node.f_parent.f_left=t_child;
			}
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<284>";
			f_root=t_child;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<286>";
		t_child.f_right=t_node;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<287>";
		t_node.f_parent=t_child;
		bb_std_lang.popErr();
		return 0;
	}
	public virtual int m_InsertFixup2(bb_map_Node2 t_node){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<212>";
		while(((t_node.f_parent)!=null) && t_node.f_parent.f_color==-1 && ((t_node.f_parent.f_parent)!=null)){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<213>";
			if(t_node.f_parent==t_node.f_parent.f_parent.f_left){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<214>";
				bb_map_Node2 t_uncle=t_node.f_parent.f_parent.f_right;
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<215>";
				if(((t_uncle)!=null) && t_uncle.f_color==-1){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<216>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<217>";
					t_uncle.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<218>";
					t_uncle.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<219>";
					t_node=t_uncle.f_parent;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<221>";
					if(t_node==t_node.f_parent.f_right){
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<222>";
						t_node=t_node.f_parent;
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<223>";
						m_RotateLeft2(t_node);
					}
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<225>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<226>";
					t_node.f_parent.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<227>";
					m_RotateRight2(t_node.f_parent.f_parent);
				}
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<230>";
				bb_map_Node2 t_uncle2=t_node.f_parent.f_parent.f_left;
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<231>";
				if(((t_uncle2)!=null) && t_uncle2.f_color==-1){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<232>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<233>";
					t_uncle2.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<234>";
					t_uncle2.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<235>";
					t_node=t_uncle2.f_parent;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<237>";
					if(t_node==t_node.f_parent.f_left){
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<238>";
						t_node=t_node.f_parent;
						bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<239>";
						m_RotateRight2(t_node);
					}
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<241>";
					t_node.f_parent.f_color=1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<242>";
					t_node.f_parent.f_parent.f_color=-1;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<243>";
					m_RotateLeft2(t_node.f_parent.f_parent);
				}
			}
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<247>";
		f_root.f_color=1;
		bb_std_lang.popErr();
		return 0;
	}
	public virtual bool m_Set2(String t_key,String t_value){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<29>";
		bb_map_Node2 t_node=f_root;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<30>";
		bb_map_Node2 t_parent=null;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<30>";
		int t_cmp=0;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<32>";
		while((t_node)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<33>";
			t_parent=t_node;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<34>";
			t_cmp=m_Compare(t_key,t_node.f_key);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<35>";
			if(t_cmp>0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<36>";
				t_node=t_node.f_right;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<37>";
				if(t_cmp<0){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<38>";
					t_node=t_node.f_left;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<40>";
					t_node.f_value=t_value;
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<41>";
					bb_std_lang.popErr();
					return false;
				}
			}
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<45>";
		t_node=(new bb_map_Node2()).g_Node_new(t_key,t_value,-1,t_parent);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<47>";
		if((t_parent)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<48>";
			if(t_cmp>0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<49>";
				t_parent.f_right=t_node;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<51>";
				t_parent.f_left=t_node;
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<53>";
			m_InsertFixup2(t_node);
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<55>";
			f_root=t_node;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<57>";
		bb_std_lang.popErr();
		return true;
	}
	public virtual bb_map_Node2 m_FindNode(String t_key){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<157>";
		bb_map_Node2 t_node=f_root;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<159>";
		while((t_node)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<160>";
			int t_cmp=m_Compare(t_key,t_node.f_key);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<161>";
			if(t_cmp>0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<162>";
				t_node=t_node.f_right;
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<163>";
				if(t_cmp<0){
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<164>";
					t_node=t_node.f_left;
				}else{
					bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<166>";
					bb_std_lang.popErr();
					return t_node;
				}
			}
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<169>";
		bb_std_lang.popErr();
		return t_node;
	}
	public virtual bool m_Contains(String t_key){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<25>";
		bool t_=m_FindNode(t_key)!=null;
		bb_std_lang.popErr();
		return t_;
	}
	public virtual String m_Get(String t_key){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<101>";
		bb_map_Node2 t_node=m_FindNode(t_key);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<102>";
		if((t_node)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<102>";
			bb_std_lang.popErr();
			return t_node.f_value;
		}
		bb_std_lang.popErr();
		return "";
	}
}
class bb_map_StringMap3 : bb_map_Map3{
	public virtual bb_map_StringMap3 g_StringMap_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<551>";
		base.g_Map_new();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<551>";
		bb_std_lang.popErr();
		return this;
	}
	public override int m_Compare(String t_lhs,String t_rhs){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<554>";
		int t_=t_lhs.CompareTo(t_rhs);
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_controls_Controls : Object{
	public static bb_virtualstick_MyStick g_TCMove;
	public static bb_touchbutton_TouchButton g_TCAction1;
	public static bb_touchbutton_TouchButton g_TCAction2;
	public static bb_touchbutton_TouchButton g_TCEscapeKey;
	public static bb_touchbutton_TouchButton[] g_TCButtons;
	public static void g_Init(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<56>";
		g_TCMove=(new bb_virtualstick_MyStick()).g_MyStick_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<57>";
		g_TCMove.m_SetRing(50.0f,(float)(bb__LDApp.g_ScreenHeight-50),40.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<58>";
		g_TCMove.m_SetStick(0.0f,0.0f,15.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<59>";
		g_TCMove.m_SetDeadZone(0.2f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<60>";
		g_TCMove.m_SetTriggerDistance(5.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<62>";
		g_TCAction1=(new bb_touchbutton_TouchButton()).g_TouchButton_new(bb__LDApp.g_ScreenWidth-60,bb__LDApp.g_ScreenHeight-40,20,20);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<63>";
		g_TCAction2=(new bb_touchbutton_TouchButton()).g_TouchButton_new(bb__LDApp.g_ScreenWidth-30,bb__LDApp.g_ScreenHeight-40,20,20);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<64>";
		g_TCEscapeKey=(new bb_touchbutton_TouchButton()).g_TouchButton_new(bb__LDApp.g_ScreenWidth-20,0,20,20);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<66>";
		g_TCButtons=new bb_touchbutton_TouchButton[3];
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<67>";
		g_TCButtons[0]=g_TCAction1;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<68>";
		g_TCButtons[1]=g_TCAction2;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<69>";
		g_TCButtons[2]=g_TCEscapeKey;
		bb_std_lang.popErr();
	}
	public static int g_ControlMethod;
	public static bool g_LeftHit;
	public static bool g_RightHit;
	public static bool g_DownHit;
	public static bool g_UpHit;
	public static bool g_ActionHit;
	public static bool g_Action2Hit;
	public static bool g_EscapeHit;
	public static int g_LeftKey;
	public static bool g_LeftDown;
	public static int g_RightKey;
	public static bool g_RightDown;
	public static int g_UpKey;
	public static bool g_UpDown;
	public static int g_DownKey;
	public static bool g_DownDown;
	public static int g_ActionKey;
	public static bool g_ActionDown;
	public static int g_Action2Key;
	public static bool g_Action2Down;
	public static int g_EscapeKey;
	public static bool g_EscapeDown;
	public static void g_UpdateKeyboard(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<97>";
		if((bb_input.bb_input_KeyDown(g_LeftKey))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<98>";
			if(g_LeftDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<99>";
				g_LeftHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<101>";
			g_LeftDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<103>";
			g_LeftDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<106>";
		if((bb_input.bb_input_KeyDown(g_RightKey))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<107>";
			if(g_RightDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<108>";
				g_RightHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<110>";
			g_RightDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<112>";
			g_RightDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<115>";
		if((bb_input.bb_input_KeyDown(g_UpKey))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<116>";
			if(g_UpDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<117>";
				g_UpHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<119>";
			g_UpDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<121>";
			g_UpDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<124>";
		if((bb_input.bb_input_KeyDown(g_DownKey))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<125>";
			if(g_DownDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<126>";
				g_DownHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<128>";
			g_DownDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<130>";
			g_DownDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<133>";
		if((bb_input.bb_input_KeyDown(g_ActionKey))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<134>";
			if(g_ActionDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<135>";
				g_ActionHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<137>";
			g_ActionDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<139>";
			g_ActionDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<142>";
		if((bb_input.bb_input_KeyDown(g_Action2Key))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<143>";
			if(g_Action2Down==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<144>";
				g_Action2Hit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<146>";
			g_Action2Down=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<148>";
			g_Action2Down=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<151>";
		if((bb_input.bb_input_KeyDown(g_EscapeKey))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<152>";
			if(g_EscapeDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<153>";
				g_EscapeHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<155>";
			g_EscapeDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<157>";
			g_EscapeDown=false;
		}
		bb_std_lang.popErr();
	}
	public static void g_UpdateJoypad(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<162>";
		if(bb_input.bb_input_JoyX(0,0)<-0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<163>";
			if(g_LeftDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<164>";
				g_LeftHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<166>";
			g_LeftDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<168>";
			g_LeftDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<171>";
		if(bb_input.bb_input_JoyX(0,0)>0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<172>";
			if(g_RightDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<173>";
				g_RightHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<175>";
			g_RightDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<177>";
			g_RightDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<180>";
		if(bb_input.bb_input_JoyY(0,0)>0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<181>";
			if(g_UpDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<182>";
				g_UpHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<184>";
			g_UpDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<186>";
			g_UpDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<189>";
		if(bb_input.bb_input_JoyY(0,0)<-0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<190>";
			if(g_DownDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<191>";
				g_DownHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<193>";
			g_DownDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<195>";
			g_DownDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<198>";
		if((bb_input.bb_input_JoyDown(0,0))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<199>";
			if(g_ActionDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<200>";
				g_ActionHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<202>";
			g_ActionDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<204>";
			g_ActionDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<207>";
		if((bb_input.bb_input_JoyDown(1,0))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<208>";
			if(g_Action2Down==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<209>";
				g_Action2Hit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<211>";
			g_Action2Down=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<213>";
			g_Action2Down=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<216>";
		if((bb_input.bb_input_JoyDown(7,0))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<217>";
			if(g_EscapeDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<218>";
				g_EscapeHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<220>";
			g_EscapeDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<222>";
			g_EscapeDown=false;
		}
		bb_std_lang.popErr();
	}
	public static bool g_TouchPoint;
	public static void g_UpdateTouch(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<228>";
		g_TCAction1.f_Hit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<229>";
		g_TCAction2.f_Hit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<230>";
		g_TCEscapeKey.f_Hit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<258>";
		if((bb_input.bb_input_MouseHit(0))!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<259>";
			g_TouchPoint=true;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<260>";
			g_TCMove.m_StartTouch(bb_autofit.bb_autofit_VMouseX(true),bb_autofit.bb_autofit_VMouseY(true),0);
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<261>";
			for(int t_i=0;t_i<=2;t_i=t_i+1){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<262>";
				if(g_TCButtons[t_i].m_Check((int)(bb_autofit.bb_autofit_VMouseX(true)),(int)(bb_autofit.bb_autofit_VMouseY(true)))){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<263>";
					g_TCButtons[t_i].f_Hit=true;
				}
			}
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<266>";
			if((bb_input.bb_input_MouseDown(0))!=0){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<267>";
				g_TCMove.m_UpdateTouch(bb_autofit.bb_autofit_VMouseX(true),bb_autofit.bb_autofit_VMouseY(true),0);
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<268>";
				for(int t_i2=0;t_i2<=2;t_i2=t_i2+1){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<269>";
					if(g_TCButtons[t_i2].m_Check((int)(bb_autofit.bb_autofit_VMouseX(true)),(int)(bb_autofit.bb_autofit_VMouseY(true)))){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<270>";
						g_TCButtons[t_i2].f_Down=true;
					}
				}
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<273>";
				if(g_TouchPoint){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<274>";
					g_TouchPoint=false;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<275>";
					g_TCMove.m_StopTouch(0);
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<276>";
					for(int t_i3=0;t_i3<=2;t_i3=t_i3+1){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<277>";
						g_TCButtons[t_i3].f_Down=false;
					}
				}
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<282>";
		if(g_TCMove.m_GetDX()<-0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<283>";
			if(g_LeftDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<284>";
				g_LeftHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<286>";
			g_LeftDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<288>";
			g_LeftDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<291>";
		if(g_TCMove.m_GetDX()>0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<292>";
			if(g_RightDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<293>";
				g_RightHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<295>";
			g_RightDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<297>";
			g_RightDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<300>";
		if(g_TCMove.m_GetDY()>0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<301>";
			if(g_UpDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<302>";
				g_UpHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<304>";
			g_UpDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<306>";
			g_UpDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<309>";
		if(g_TCMove.m_GetDY()<-0.1f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<310>";
			if(g_DownDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<311>";
				g_DownHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<313>";
			g_DownDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<315>";
			g_DownDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<318>";
		if(g_TCAction1.f_Down){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<319>";
			if(g_ActionDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<320>";
				g_ActionHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<322>";
			g_ActionDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<324>";
			g_ActionDown=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<327>";
		if(g_TCAction2.f_Down){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<328>";
			if(g_Action2Down==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<329>";
				g_Action2Hit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<331>";
			g_Action2Down=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<333>";
			g_Action2Down=false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<336>";
		if(g_TCEscapeKey.f_Down){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<337>";
			if(g_EscapeDown==false){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<338>";
				g_EscapeHit=true;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<340>";
			g_EscapeDown=true;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<342>";
			g_EscapeDown=false;
		}
		bb_std_lang.popErr();
	}
	public static void g_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<75>";
		g_LeftHit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<76>";
		g_RightHit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<77>";
		g_DownHit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<78>";
		g_UpHit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<79>";
		g_ActionHit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<80>";
		g_Action2Hit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<81>";
		g_EscapeHit=false;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<83>";
		int t_=g_ControlMethod;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<84>";
		if(t_==0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<85>";
			g_UpdateKeyboard();
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<86>";
			if(t_==1){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<87>";
				g_UpdateJoypad();
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<88>";
				if(t_==2){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/controls.monkey<89>";
					g_UpdateTouch();
				}
			}
		}
		bb_std_lang.popErr();
	}
}
class bb_virtualstick_VirtualStick : Object{
	public virtual bb_virtualstick_VirtualStick g_VirtualStick_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<3>";
		bb_std_lang.popErr();
		return this;
	}
	public float f_ringX=.0f;
	public float f_ringY=.0f;
	public float f_ringRadius=.0f;
	public virtual void m_SetRing(float t_ringX,float t_ringY,float t_ringRadius){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<160>";
		this.f_ringX=t_ringX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<161>";
		this.f_ringY=t_ringY;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<162>";
		this.f_ringRadius=t_ringRadius;
		bb_std_lang.popErr();
	}
	public float f_stickX=0.0f;
	public float f_stickY=0.0f;
	public float f_stickRadius=.0f;
	public virtual void m_SetStick(float t_stickX,float t_stickY,float t_stickRadius){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<167>";
		this.f_stickX=t_stickX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<168>";
		this.f_stickY=t_stickY;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<169>";
		this.f_stickRadius=t_stickRadius;
		bb_std_lang.popErr();
	}
	public float f_deadZone=.0f;
	public virtual void m_SetDeadZone(float t_deadZone){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<173>";
		this.f_deadZone=t_deadZone;
		bb_std_lang.popErr();
	}
	public float f_triggerDistance=-1.0f;
	public virtual void m_SetTriggerDistance(float t_triggerDistance){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<177>";
		this.f_triggerDistance=t_triggerDistance;
		bb_std_lang.popErr();
	}
	public int f_touchNumber=-1;
	public float f_firstTouchX=.0f;
	public float f_firstTouchY=.0f;
	public bool f_triggered=false;
	public float f_stickPower=.0f;
	public float f_stickAngle=.0f;
	public virtual void m_UpdateStick(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<34>";
		if(f_touchNumber>=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<35>";
			float t_length=(float)Math.Sqrt(f_stickX*f_stickX+f_stickY*f_stickY);
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<36>";
			f_stickPower=t_length/f_ringRadius;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<37>";
			if(f_stickPower>1.0f){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<37>";
				f_stickPower=1.0f;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<39>";
			if(f_stickPower<f_deadZone){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<40>";
				f_stickPower=0.0f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<41>";
				f_stickAngle=0.0f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<42>";
				f_stickX=0.0f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<43>";
				f_stickY=0.0f;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<45>";
				if(f_stickX==0.0f && f_stickY==0.0f){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<46>";
					f_stickAngle=0.0f;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<47>";
					f_stickPower=0.0f;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<48>";
					if(f_stickX==0.0f && f_stickY>0.0f){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<49>";
						f_stickAngle=90.0f;
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<50>";
						if(f_stickX==0.0f && f_stickY<0.0f){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<51>";
							f_stickAngle=270.0f;
						}else{
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<52>";
							if(f_stickY==0.0f && f_stickX>0.0f){
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<53>";
								f_stickAngle=0.0f;
							}else{
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<54>";
								if(f_stickY==0.0f && f_stickX<0.0f){
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<55>";
									f_stickAngle=180.0f;
								}else{
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<56>";
									if(f_stickX>0.0f && f_stickY>0.0f){
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<57>";
										f_stickAngle=(float)(Math.Atan(f_stickY/f_stickX)*bb_std_lang.R2D);
									}else{
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<58>";
										if(f_stickX<0.0f){
											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<59>";
											f_stickAngle=180.0f+(float)(Math.Atan(f_stickY/f_stickX)*bb_std_lang.R2D);
										}else{
											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<61>";
											f_stickAngle=360.0f+(float)(Math.Atan(f_stickY/f_stickX)*bb_std_lang.R2D);
										}
									}
								}
							}
						}
					}
				}
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<63>";
				if(t_length>f_ringRadius){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<64>";
					f_stickPower=1.0f;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<65>";
					f_stickX=(float)Math.Cos((f_stickAngle)*bb_std_lang.D2R)*f_ringRadius;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<66>";
					f_stickY=(float)Math.Sin((f_stickAngle)*bb_std_lang.D2R)*f_ringRadius;
				}
			}
		}
		bb_std_lang.popErr();
	}
	public virtual void m_StartTouch(float t_x,float t_y,int t_touchnum){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<96>";
		if(f_touchNumber<0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<97>";
			if((t_x-f_ringX)*(t_x-f_ringX)+(t_y-f_ringY)*(t_y-f_ringY)<=f_ringRadius*f_ringRadius){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<98>";
				f_touchNumber=t_touchnum;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<99>";
				f_firstTouchX=t_x;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<100>";
				f_firstTouchY=t_y;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<101>";
				f_triggered=false;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<102>";
				if(f_triggerDistance<=0.0f){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<103>";
					f_triggered=true;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<104>";
					f_stickX=t_x-f_ringX;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<105>";
					f_stickY=f_ringY-t_y;
				}
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<107>";
				m_UpdateStick();
			}
		}
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateTouch(float t_x,float t_y,int t_touchnum){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<114>";
		if(f_touchNumber>=0 && f_touchNumber==t_touchnum){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<115>";
			if(!f_triggered){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<116>";
				if((t_x-f_firstTouchX)*(t_x-f_firstTouchX)+(t_y-f_firstTouchY)*(t_y-f_firstTouchY)>f_triggerDistance*f_triggerDistance){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<117>";
					f_triggered=true;
				}
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<120>";
			if(f_triggered){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<121>";
				f_stickX=t_x-f_ringX;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<122>";
				f_stickY=f_ringY-t_y;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<123>";
				m_UpdateStick();
			}
		}
		bb_std_lang.popErr();
	}
	public virtual void m_StopTouch(int t_touchnum){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<130>";
		if(f_touchNumber>=0 && f_touchNumber==t_touchnum){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<131>";
			f_touchNumber=-1;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<132>";
			f_stickX=0.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<133>";
			f_stickY=0.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<134>";
			f_stickAngle=0.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<135>";
			f_stickPower=0.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<136>";
			f_triggered=false;
		}
		bb_std_lang.popErr();
	}
	public virtual float m_GetDX(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<86>";
		float t_=(float)Math.Cos((f_stickAngle)*bb_std_lang.D2R)*f_stickPower;
		bb_std_lang.popErr();
		return t_;
	}
	public virtual float m_GetDY(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<91>";
		float t_=(float)Math.Sin((f_stickAngle)*bb_std_lang.D2R)*f_stickPower;
		bb_std_lang.popErr();
		return t_;
	}
	public virtual void m_RenderRing(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<155>";
		bb_graphics.bb_graphics_DrawCircle(t_x,t_y,f_ringRadius);
		bb_std_lang.popErr();
	}
	public virtual void m_DoRenderRing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<141>";
		m_RenderRing(f_ringX,f_ringY);
		bb_std_lang.popErr();
	}
	public virtual void m_RenderStick(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<150>";
		bb_graphics.bb_graphics_DrawCircle(t_x,t_y,f_stickRadius);
		bb_std_lang.popErr();
	}
	public virtual void m_DoRenderStick(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<145>";
		m_RenderStick(f_ringX+f_stickX,f_ringY-f_stickY);
		bb_std_lang.popErr();
	}
}
class bb_virtualstick_MyStick : bb_virtualstick_VirtualStick{
	public virtual bb_virtualstick_MyStick g_MyStick_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<181>";
		base.g_VirtualStick_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<181>";
		bb_std_lang.popErr();
		return this;
	}
	public override void m_RenderRing(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<183>";
		bb_graphics.bb_graphics_SetColor(0.0f,0.0f,0.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<184>";
		bb_graphics.bb_graphics_SetAlpha(0.1f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<185>";
		base.m_RenderRing(t_x,t_y);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<186>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.popErr();
	}
	public override void m_RenderStick(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<190>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<191>";
		bb_graphics.bb_graphics_SetAlpha(0.5f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<193>";
		base.m_RenderStick(t_x,t_y);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/virtualstick.monkey<194>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.popErr();
	}
}
class bb_touchbutton_TouchButton : Object{
	public int f_X=0;
	public int f_Y=0;
	public int f_W=0;
	public int f_H=0;
	public virtual bb_touchbutton_TouchButton g_TouchButton_new(int t_tX,int t_tY,int t_tW,int t_tH){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<24>";
		f_X=t_tX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<25>";
		f_Y=t_tY;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<26>";
		f_W=t_tW;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<27>";
		f_H=t_tH;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_touchbutton_TouchButton g_TouchButton_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<3>";
		bb_std_lang.popErr();
		return this;
	}
	public bool f_Hit=false;
	public virtual bool m_Check(int t_tX,int t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<31>";
		bool t_=bb_functions.bb_functions_PointInRect((float)(t_tX),(float)(t_tY),(float)(f_X),(float)(f_Y),(float)(f_W),(float)(f_H));
		bb_std_lang.popErr();
		return t_;
	}
	public bool f_Down=false;
	public virtual void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<35>";
		bb_graphics.bb_graphics_SetColor(0.0f,0.0f,0.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<36>";
		bb_graphics.bb_graphics_SetAlpha(0.25f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<37>";
		bb_graphics.bb_graphics_DrawRect((float)(f_X),(float)(f_Y),(float)(f_W),(float)(f_H));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<39>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<40>";
		bb_graphics.bb_graphics_SetAlpha(0.5f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/touchbutton.monkey<41>";
		bb_gfx.bb_gfx_DrawHollowRect(f_X,f_Y,f_W,f_H);
		bb_std_lang.popErr();
	}
}
class bb_raztext_RazText : Object{
	public static bb_graphics_Image g_TextSheet;
	public static void g_SetTextSheet(bb_graphics_Image t_tImage){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<8>";
		g_TextSheet=t_tImage;
		bb_std_lang.popErr();
	}
	public bb_list_List f_Lines=null;
	public virtual bb_raztext_RazText g_RazText_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<27>";
		f_Lines=(new bb_list_List()).g_List_new();
		bb_std_lang.popErr();
		return this;
	}
	public String f_OriginalString="";
	public virtual void m_AddLine(String t_tString){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<256>";
		bb_raztext_RazChar[] t_tmp=new bb_raztext_RazChar[t_tString.Length];
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<257>";
		for(int t_i=0;t_i<=t_tString.Length-1;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<258>";
			int t_let=(int)t_tString[t_i];
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<259>";
			String t_letstring=((String)bb_std_lang.slice(t_tString,t_i,t_i+1)).ToLower();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<260>";
			int t_XOff=0;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<261>";
			int t_YOff=0;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<262>";
			String t_=t_letstring.ToLower();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<263>";
			if(t_.CompareTo("a")==0){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<264>";
				t_XOff=0;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<265>";
				t_YOff=1;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<266>";
				if(t_.CompareTo("b")==0){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<267>";
					t_XOff=1;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<268>";
					t_YOff=1;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<269>";
					if(t_.CompareTo("c")==0){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<270>";
						t_XOff=2;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<271>";
						t_YOff=1;
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<272>";
						if(t_.CompareTo("d")==0){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<273>";
							t_XOff=3;
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<274>";
							t_YOff=1;
						}else{
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<275>";
							if(t_.CompareTo("e")==0){
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<276>";
								t_XOff=4;
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<277>";
								t_YOff=1;
							}else{
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<278>";
								if(t_.CompareTo("f")==0){
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<279>";
									t_XOff=5;
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<280>";
									t_YOff=1;
								}else{
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<281>";
									if(t_.CompareTo("g")==0){
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<282>";
										t_XOff=6;
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<283>";
										t_YOff=1;
									}else{
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<284>";
										if(t_.CompareTo("h")==0){
											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<285>";
											t_XOff=7;
											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<286>";
											t_YOff=1;
										}else{
											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<287>";
											if(t_.CompareTo("i")==0){
												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<288>";
												t_XOff=8;
												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<289>";
												t_YOff=1;
											}else{
												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<290>";
												if(t_.CompareTo("j")==0){
													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<291>";
													t_XOff=9;
													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<292>";
													t_YOff=1;
												}else{
													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<293>";
													if(t_.CompareTo("k")==0){
														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<294>";
														t_XOff=0;
														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<295>";
														t_YOff=2;
													}else{
														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<296>";
														if(t_.CompareTo("l")==0){
															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<297>";
															t_XOff=1;
															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<298>";
															t_YOff=2;
														}else{
															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<299>";
															if(t_.CompareTo("m")==0){
																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<300>";
																t_XOff=2;
																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<301>";
																t_YOff=2;
															}else{
																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<302>";
																if(t_.CompareTo("n")==0){
																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<303>";
																	t_XOff=3;
																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<304>";
																	t_YOff=2;
																}else{
																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<305>";
																	if(t_.CompareTo("o")==0){
																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<306>";
																		t_XOff=4;
																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<307>";
																		t_YOff=2;
																	}else{
																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<308>";
																		if(t_.CompareTo("p")==0){
																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<309>";
																			t_XOff=5;
																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<310>";
																			t_YOff=2;
																		}else{
																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<311>";
																			if(t_.CompareTo("q")==0){
																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<312>";
																				t_XOff=6;
																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<313>";
																				t_YOff=2;
																			}else{
																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<314>";
																				if(t_.CompareTo("r")==0){
																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<315>";
																					t_XOff=7;
																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<316>";
																					t_YOff=2;
																				}else{
																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<317>";
																					if(t_.CompareTo("s")==0){
																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<318>";
																						t_XOff=8;
																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<319>";
																						t_YOff=2;
																					}else{
																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<320>";
																						if(t_.CompareTo("t")==0){
																							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<321>";
																							t_XOff=9;
																							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<322>";
																							t_YOff=2;
																						}else{
																							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<323>";
																							if(t_.CompareTo("u")==0){
																								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<324>";
																								t_XOff=0;
																								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<325>";
																								t_YOff=3;
																							}else{
																								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<326>";
																								if(t_.CompareTo("v")==0){
																									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<327>";
																									t_XOff=1;
																									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<328>";
																									t_YOff=3;
																								}else{
																									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<329>";
																									if(t_.CompareTo("w")==0){
																										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<330>";
																										t_XOff=2;
																										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<331>";
																										t_YOff=3;
																									}else{
																										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<332>";
																										if(t_.CompareTo("x")==0){
																											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<333>";
																											t_XOff=3;
																											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<334>";
																											t_YOff=3;
																										}else{
																											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<335>";
																											if(t_.CompareTo("y")==0){
																												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<336>";
																												t_XOff=4;
																												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<337>";
																												t_YOff=3;
																											}else{
																												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<338>";
																												if(t_.CompareTo("z")==0){
																													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<339>";
																													t_XOff=5;
																													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<340>";
																													t_YOff=3;
																												}else{
																													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<341>";
																													if(t_.CompareTo("0")==0){
																														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<342>";
																														t_XOff=9;
																														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<343>";
																														t_YOff=0;
																													}else{
																														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<344>";
																														if(t_.CompareTo("1")==0){
																															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<345>";
																															t_XOff=0;
																															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<346>";
																															t_YOff=0;
																														}else{
																															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<347>";
																															if(t_.CompareTo("2")==0){
																																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<348>";
																																t_XOff=1;
																																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<349>";
																																t_YOff=0;
																															}else{
																																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<350>";
																																if(t_.CompareTo("3")==0){
																																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<351>";
																																	t_XOff=2;
																																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<352>";
																																	t_YOff=0;
																																}else{
																																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<353>";
																																	if(t_.CompareTo("4")==0){
																																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<354>";
																																		t_XOff=3;
																																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<355>";
																																		t_YOff=0;
																																	}else{
																																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<356>";
																																		if(t_.CompareTo("5")==0){
																																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<357>";
																																			t_XOff=4;
																																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<358>";
																																			t_YOff=0;
																																		}else{
																																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<359>";
																																			if(t_.CompareTo("6")==0){
																																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<360>";
																																				t_XOff=5;
																																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<361>";
																																				t_YOff=0;
																																			}else{
																																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<362>";
																																				if(t_.CompareTo("7")==0){
																																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<363>";
																																					t_XOff=6;
																																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<364>";
																																					t_YOff=0;
																																				}else{
																																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<365>";
																																					if(t_.CompareTo("8")==0){
																																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<366>";
																																						t_XOff=7;
																																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<367>";
																																						t_YOff=0;
																																					}else{
																																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<368>";
																																						if(t_.CompareTo("9")==0){
																																							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<369>";
																																							t_XOff=8;
																																							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<370>";
																																							t_YOff=0;
																																						}else{
																																							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<371>";
																																							if(t_.CompareTo(",")==0){
																																								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<372>";
																																								t_XOff=6;
																																								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<373>";
																																								t_YOff=3;
																																							}else{
																																								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<374>";
																																								if(t_.CompareTo(".")==0){
																																									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<375>";
																																									t_XOff=7;
																																									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<376>";
																																									t_YOff=3;
																																								}else{
																																									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<377>";
																																									if(t_.CompareTo("!")==0){
																																										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<378>";
																																										t_XOff=8;
																																										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<379>";
																																										t_YOff=3;
																																									}else{
																																										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<380>";
																																										if(t_.CompareTo("?")==0){
																																											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<381>";
																																											t_XOff=9;
																																											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<382>";
																																											t_YOff=3;
																																										}else{
																																											bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<383>";
																																											if(t_.CompareTo("@")==0){
																																												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<384>";
																																												t_XOff=1;
																																												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<385>";
																																												t_YOff=4;
																																											}else{
																																												bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<386>";
																																												if(t_.CompareTo(" ")==0){
																																													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<387>";
																																													t_XOff=9;
																																													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<388>";
																																													t_YOff=4;
																																												}else{
																																													bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<389>";
																																													if(t_.CompareTo("/")==0){
																																														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<390>";
																																														t_XOff=0;
																																														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<391>";
																																														t_YOff=4;
																																													}else{
																																														bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<392>";
																																														if(t_.CompareTo("-")==0){
																																															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<393>";
																																															t_XOff=2;
																																															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<394>";
																																															t_YOff=4;
																																														}else{
																																															bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<395>";
																																															if(t_.CompareTo(":")==0){
																																																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<396>";
																																																t_XOff=3;
																																																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<397>";
																																																t_YOff=4;
																																															}else{
																																																bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<398>";
																																																if(t_.CompareTo(";")==0){
																																																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<399>";
																																																	t_XOff=4;
																																																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<400>";
																																																	t_YOff=4;
																																																}else{
																																																	bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<401>";
																																																	if(t_.CompareTo("_")==0){
																																																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<402>";
																																																		t_XOff=5;
																																																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<403>";
																																																		t_YOff=4;
																																																	}else{
																																																		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<404>";
																																																		if((t_.CompareTo("(")==0)){
																																																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<405>";
																																																			t_XOff=6;
																																																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<406>";
																																																			t_YOff=4;
																																																		}else{
																																																			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<407>";
																																																			if((t_.CompareTo(")")==0)){
																																																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<408>";
																																																				t_XOff=7;
																																																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<409>";
																																																				t_YOff=4;
																																																			}else{
																																																				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<410>";
																																																				if(t_.CompareTo("*")==0){
																																																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<411>";
																																																					t_XOff=8;
																																																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<412>";
																																																					t_YOff=4;
																																																				}else{
																																																					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<413>";
																																																					if(t_.CompareTo("+")==0){
																																																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<414>";
																																																						t_XOff=9;
																																																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<415>";
																																																						t_YOff=4;
																																																					}else{
																																																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<417>";
																																																						t_XOff=9;
																																																						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<418>";
																																																						t_YOff=4;
																																																					}
																																																				}
																																																			}
																																																		}
																																																	}
																																																}
																																															}
																																														}
																																													}
																																												}
																																											}
																																										}
																																									}
																																								}
																																							}
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<422>";
			t_tmp[t_i]=(new bb_raztext_RazChar()).g_RazChar_new();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<423>";
			t_tmp[t_i].f_XOff=t_XOff;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<424>";
			t_tmp[t_i].f_YOff=t_YOff;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<425>";
			t_tmp[t_i].f_TextValue=t_letstring;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<426>";
			if(t_XOff==9 && t_YOff==4){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<427>";
				t_tmp[t_i].f_Visible=false;
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<432>";
		f_Lines.m_AddLast(t_tmp);
		bb_std_lang.popErr();
	}
	public virtual bb_raztext_RazText g_RazText_new2(String t_tString){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<31>";
		f_Lines=(new bb_list_List()).g_List_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<32>";
		f_OriginalString=t_tString;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<33>";
		m_AddLine(t_tString);
		bb_std_lang.popErr();
		return this;
	}
	public virtual void m_AddMutliLines(String t_tString){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<438>";
		String[] t_tmp=bb_std_lang.split(t_tString,"\r");
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<439>";
		for(int t_i=0;t_i<=bb_std_lang.length(t_tmp)-1;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<440>";
			m_AddLine(t_tmp[t_i]);
		}
		bb_std_lang.popErr();
	}
	public int f_X=0;
	public int f_Y=0;
	public virtual void m_SetPos(int t_tX,int t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<445>";
		f_X=t_tX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<446>";
		f_Y=t_tY;
		bb_std_lang.popErr();
	}
	public int f_VerticalSpacing=0;
	public int f_HorizontalSpacing=-2;
	public virtual void m_SetSpacing(int t_tX,int t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<41>";
		f_VerticalSpacing=t_tY;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<42>";
		f_HorizontalSpacing=t_tX;
		bb_std_lang.popErr();
	}
	public int f_CharacterWidth=10;
	public int f_CharacterHeight=10;
	public int f_CharacterOriginX=0;
	public int f_CharacterOriginY=0;
	public virtual void m_Draw(int t_tX,int t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<456>";
		int t_drawn=0;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<458>";
		int t_cY=t_tY;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<459>";
		int t_cX=t_tX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<460>";
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<460>";
		bb_list_Enumerator t_=f_Lines.m_ObjectEnumerator();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<460>";
		while(t_.m_HasNext()){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<460>";
			bb_raztext_RazChar[] t_tLine=t_.m_NextObject();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<461>";
			for(int t_i=0;t_i<=bb_std_lang.length(t_tLine)-1;t_i=t_i+1){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<462>";
				if(t_tLine[t_i].f_Visible==true){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<463>";
					if(bb_functions.bb_functions_RectOverRect((float)(t_cX),(float)(t_cY),(float)(f_CharacterWidth),(float)(f_CharacterHeight),0.0f,0.0f,(float)(bb__LDApp.g_ScreenWidth),(float)(bb__LDApp.g_ScreenHeight))==true){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<464>";
						t_drawn+=1;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<465>";
						bb_graphics.bb_graphics_DrawImageRect(g_TextSheet,(float)(t_cX),(float)(t_cY),f_CharacterOriginX+t_tLine[t_i].f_XOff*f_CharacterWidth,f_CharacterOriginY+t_tLine[t_i].f_YOff*f_CharacterHeight,f_CharacterWidth,f_CharacterHeight,0);
					}
				}
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<468>";
				t_cX+=f_CharacterWidth+f_HorizontalSpacing;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<471>";
			t_cY+=f_CharacterHeight+f_VerticalSpacing;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<472>";
			t_cX=t_tX;
		}
		bb_std_lang.popErr();
	}
	public virtual void m_Draw2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<451>";
		m_Draw(f_X,f_Y);
		bb_std_lang.popErr();
	}
}
class bb_gamescreen_GameScreen : bb_screen_Screen{
	public virtual bb_gamescreen_GameScreen g_GameScreen_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<33>";
		base.g_Screen_new();
		bb_std_lang.popErr();
		return this;
	}
	public bb_level_Level f_level=null;
	public override void m_OnScreenStart(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<8>";
		bb__LDApp.g_level=(new bb_level_Level()).g_Level_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<9>";
		f_level=bb__LDApp.g_level;
		bb_std_lang.popErr();
	}
	public override void m_OnScreenEnd(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public override void m_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<20>";
		if(f_level!=null){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<21>";
			f_level.m_Update();
		}
		bb_std_lang.popErr();
	}
	public override void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<27>";
		bb_graphics.bb_graphics_Cls(255.0f,255.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<28>";
		if(f_level!=null){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/screens/gamescreen.monkey<29>";
			f_level.m_Render();
		}
		bb_std_lang.popErr();
	}
}
class bb_map_Node : Object{
	public String f_key="";
	public bb_map_Node f_right=null;
	public bb_map_Node f_left=null;
	public bb_screen_Screen f_value=null;
	public int f_color=0;
	public bb_map_Node f_parent=null;
	public virtual bb_map_Node g_Node_new(String t_key,bb_screen_Screen t_value,int t_color,bb_map_Node t_parent){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<364>";
		this.f_key=t_key;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<365>";
		this.f_value=t_value;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<366>";
		this.f_color=t_color;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<367>";
		this.f_parent=t_parent;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_map_Node g_Node_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<361>";
		bb_std_lang.popErr();
		return this;
	}
}
class bb_map_Node2 : Object{
	public String f_key="";
	public bb_map_Node2 f_right=null;
	public bb_map_Node2 f_left=null;
	public String f_value="";
	public int f_color=0;
	public bb_map_Node2 f_parent=null;
	public virtual bb_map_Node2 g_Node_new(String t_key,String t_value,int t_color,bb_map_Node2 t_parent){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<364>";
		this.f_key=t_key;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<365>";
		this.f_value=t_value;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<366>";
		this.f_color=t_color;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<367>";
		this.f_parent=t_parent;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_map_Node2 g_Node_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/map.monkey<361>";
		bb_std_lang.popErr();
		return this;
	}
}
class bb_controls_ControlMethodTypes : Object{
}
class bb_autofit_VirtualDisplay : Object{
	public float f_vwidth=.0f;
	public float f_vheight=.0f;
	public float f_vzoom=.0f;
	public float f_lastvzoom=.0f;
	public float f_vratio=.0f;
	public static bb_autofit_VirtualDisplay g_Display;
	public virtual bb_autofit_VirtualDisplay g_VirtualDisplay_new(int t_width,int t_height,float t_zoom){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<266>";
		f_vwidth=(float)(t_width);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<267>";
		f_vheight=(float)(t_height);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<269>";
		f_vzoom=t_zoom;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<270>";
		f_lastvzoom=f_vzoom+1.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<274>";
		f_vratio=f_vheight/f_vwidth;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<278>";
		g_Display=this;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_autofit_VirtualDisplay g_VirtualDisplay_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<216>";
		bb_std_lang.popErr();
		return this;
	}
	public float f_multi=.0f;
	public virtual float m_VMouseX(bool t_limit){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<302>";
		float t_mouseoffset=bb_input.bb_input_MouseX()-(float)(bb_graphics.bb_graphics_DeviceWidth())*0.5f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<306>";
		float t_x=t_mouseoffset/f_multi/f_vzoom+bb_autofit.bb_autofit_VDeviceWidth()*0.5f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<310>";
		if(t_limit){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<312>";
			float t_widthlimit=f_vwidth-1.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<314>";
			if(t_x>0.0f){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<315>";
				if(t_x<t_widthlimit){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<316>";
					bb_std_lang.popErr();
					return t_x;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<318>";
					bb_std_lang.popErr();
					return t_widthlimit;
				}
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<321>";
				bb_std_lang.popErr();
				return 0.0f;
			}
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<325>";
			bb_std_lang.popErr();
			return t_x;
		}
	}
	public virtual float m_VMouseY(bool t_limit){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<336>";
		float t_mouseoffset=bb_input.bb_input_MouseY()-(float)(bb_graphics.bb_graphics_DeviceHeight())*0.5f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<340>";
		float t_y=t_mouseoffset/f_multi/f_vzoom+bb_autofit.bb_autofit_VDeviceHeight()*0.5f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<344>";
		if(t_limit){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<346>";
			float t_heightlimit=f_vheight-1.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<348>";
			if(t_y>0.0f){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<349>";
				if(t_y<t_heightlimit){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<350>";
					bb_std_lang.popErr();
					return t_y;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<352>";
					bb_std_lang.popErr();
					return t_heightlimit;
				}
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<355>";
				bb_std_lang.popErr();
				return 0.0f;
			}
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<359>";
			bb_std_lang.popErr();
			return t_y;
		}
	}
	public int f_lastdevicewidth=0;
	public int f_lastdeviceheight=0;
	public int f_device_changed=0;
	public float f_fdw=.0f;
	public float f_fdh=.0f;
	public float f_dratio=.0f;
	public float f_heightborder=.0f;
	public float f_widthborder=.0f;
	public int f_zoom_changed=0;
	public float f_realx=.0f;
	public float f_realy=.0f;
	public float f_offx=.0f;
	public float f_offy=.0f;
	public float f_sx=.0f;
	public float f_sw=.0f;
	public float f_sy=.0f;
	public float f_sh=.0f;
	public float f_scaledw=.0f;
	public float f_scaledh=.0f;
	public float f_vxoff=.0f;
	public float f_vyoff=.0f;
	public virtual int m_UpdateVirtualDisplay(bool t_zoomborders,bool t_keepborders){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<444>";
		if(bb_graphics.bb_graphics_DeviceWidth()!=f_lastdevicewidth || bb_graphics.bb_graphics_DeviceHeight()!=f_lastdeviceheight){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<445>";
			f_lastdevicewidth=bb_graphics.bb_graphics_DeviceWidth();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<446>";
			f_lastdeviceheight=bb_graphics.bb_graphics_DeviceHeight();
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<447>";
			f_device_changed=1;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<454>";
		if((f_device_changed)!=0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<459>";
			f_fdw=(float)(bb_graphics.bb_graphics_DeviceWidth());
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<460>";
			f_fdh=(float)(bb_graphics.bb_graphics_DeviceHeight());
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<465>";
			f_dratio=f_fdh/f_fdw;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<469>";
			if(f_dratio>f_vratio){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<478>";
				f_multi=f_fdw/f_vwidth;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<482>";
				f_heightborder=(f_fdh-f_vheight*f_multi)*0.5f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<483>";
				f_widthborder=0.0f;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<494>";
				f_multi=f_fdh/f_vheight;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<498>";
				f_widthborder=(f_fdw-f_vwidth*f_multi)*0.5f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<499>";
				f_heightborder=0.0f;
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<509>";
		if(f_vzoom!=f_lastvzoom){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<510>";
			f_lastvzoom=f_vzoom;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<511>";
			f_zoom_changed=1;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<518>";
		if(((f_zoom_changed)!=0) || ((f_device_changed)!=0)){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<520>";
			if(t_zoomborders){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<524>";
				f_realx=f_vwidth*f_vzoom*f_multi;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<525>";
				f_realy=f_vheight*f_vzoom*f_multi;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<529>";
				f_offx=(f_fdw-f_realx)*0.5f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<530>";
				f_offy=(f_fdh-f_realy)*0.5f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<532>";
				if(t_keepborders){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<538>";
					if(f_offx<f_widthborder){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<539>";
						f_sx=f_widthborder;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<540>";
						f_sw=f_fdw-f_widthborder*2.0f;
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<542>";
						f_sx=f_offx;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<543>";
						f_sw=f_fdw-f_offx*2.0f;
					}
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<546>";
					if(f_offy<f_heightborder){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<547>";
						f_sy=f_heightborder;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<548>";
						f_sh=f_fdh-f_heightborder*2.0f;
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<550>";
						f_sy=f_offy;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<551>";
						f_sh=f_fdh-f_offy*2.0f;
					}
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<556>";
					f_sx=f_offx;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<557>";
					f_sw=f_fdw-f_offx*2.0f;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<559>";
					f_sy=f_offy;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<560>";
					f_sh=f_fdh-f_offy*2.0f;
				}
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<566>";
				f_sx=bb_math.bb_math_Max2(0.0f,f_sx);
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<567>";
				f_sy=bb_math.bb_math_Max2(0.0f,f_sy);
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<568>";
				f_sw=bb_math.bb_math_Min2(f_sw,f_fdw);
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<569>";
				f_sh=bb_math.bb_math_Min2(f_sh,f_fdh);
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<575>";
				f_sx=bb_math.bb_math_Max2(0.0f,f_widthborder);
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<576>";
				f_sy=bb_math.bb_math_Max2(0.0f,f_heightborder);
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<577>";
				f_sw=bb_math.bb_math_Min2(f_fdw-f_widthborder*2.0f,f_fdw);
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<578>";
				f_sh=bb_math.bb_math_Min2(f_fdh-f_heightborder*2.0f,f_fdh);
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<584>";
			f_scaledw=f_vwidth*f_multi*f_vzoom;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<585>";
			f_scaledh=f_vheight*f_multi*f_vzoom;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<589>";
			f_vxoff=(f_fdw-f_scaledw)*0.5f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<590>";
			f_vyoff=(f_fdh-f_scaledh)*0.5f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<594>";
			f_vxoff=f_vxoff/f_multi/f_vzoom;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<595>";
			f_vyoff=f_vyoff/f_multi/f_vzoom;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<599>";
			f_device_changed=0;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<600>";
			f_zoom_changed=0;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<608>";
		bb_graphics.bb_graphics_SetScissor(0.0f,0.0f,(float)(bb_graphics.bb_graphics_DeviceWidth()),(float)(bb_graphics.bb_graphics_DeviceHeight()));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<609>";
		bb_graphics.bb_graphics_Cls(0.0f,0.0f,0.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<615>";
		bb_graphics.bb_graphics_SetScissor(f_sx,f_sy,f_sw,f_sh);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<621>";
		bb_graphics.bb_graphics_Scale(f_multi*f_vzoom,f_multi*f_vzoom);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<627>";
		if((f_vzoom)!=0.0f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<627>";
			bb_graphics.bb_graphics_Translate(f_vxoff,f_vyoff);
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<629>";
		bb_std_lang.popErr();
		return 0;
	}
}
class bb_level_Level : Object{
	public int f_controlledYeti=0;
	public bb_raztext_RazText f_txtWait=null;
	public virtual bb_level_Level g_Level_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<11>";
		bb_dog_Dog.g_Init(bb__LDApp.g_level);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<12>";
		bb_yeti_Yeti.g_Init(bb__LDApp.g_level);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<13>";
		bb_skier_Skier.g_Init(bb__LDApp.g_level);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<15>";
		f_controlledYeti=bb_yeti_Yeti.g_Create(0.0f,0.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<16>";
		bb_yeti_Yeti.g_a[f_controlledYeti].m_StartWaiting();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<18>";
		bb_dog_Dog.g_Create(50.0f,50.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<20>";
		int t_firstSkier=bb_skier_Skier.g_Create(50.0f,-70.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<21>";
		bb_skier_Skier.g_a[t_firstSkier].m_StartTeasing();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<23>";
		bb__LDApp.g_SetScreenPosition(bb_yeti_Yeti.g_a[f_controlledYeti].f_X,bb_yeti_Yeti.g_a[f_controlledYeti].f_Y);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<25>";
		f_txtWait=(new bb_raztext_RazText()).g_RazText_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<26>";
		f_txtWait.m_AddMutliLines(bb_app.bb_app_LoadString("txt/wait.txt").Replace("\n",""));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<27>";
		f_txtWait.m_SetPos(96,320);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<28>";
		f_txtWait.m_SetSpacing(-3,-1);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<30>";
		bb_sfx_SFX.g_Music("ambient",1);
		bb_std_lang.popErr();
		return this;
	}
	public virtual void m_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<35>";
		bb__LDApp.g_SetScreenTarget(bb_yeti_Yeti.g_a[f_controlledYeti].f_X,bb_yeti_Yeti.g_a[f_controlledYeti].f_Y+(float)(bb__LDApp.g_ScreenHeight)*0.25f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<37>";
		bb_dog_Dog.g_UpdateAll();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<38>";
		bb_yeti_Yeti.g_UpdateAll();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<39>";
		bb_skier_Skier.g_UpdateAll();
		bb_std_lang.popErr();
	}
	public virtual void m_RenderGui(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<69>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<70>";
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<71>";
		bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,0.0f,0.0f,504,0,8,360,0);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<72>";
		bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,6.0f,bb_yeti_Yeti.g_a[f_controlledYeti].f_Y/50.0f,464,0,10,10,0);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<73>";
		for(int t_i=0;t_i<20;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<74>";
			if(bb_skier_Skier.g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<75>";
				bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,6.0f,bb_skier_Skier.g_a[t_i].f_Y/50.0f,480,0,10,10,0);
			}
		}
		bb_std_lang.popErr();
	}
	public virtual void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<44>";
		bb_dog_Dog.g_RenderAll();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<45>";
		bb_yeti_Yeti.g_RenderAll();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<46>";
		bb_skier_Skier.g_RenderAll();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<47>";
		f_txtWait.m_Draw2();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/level.monkey<49>";
		m_RenderGui();
		bb_std_lang.popErr();
	}
}
class bb_entity_Entity : Object{
	public bb_level_Level f_level=null;
	public float f_W=.0f;
	public float f_H=.0f;
	public virtual bb_entity_Entity g_Entity_new(bb_level_Level t_tLev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<24>";
		f_level=t_tLev;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<25>";
		f_W=16.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<26>";
		f_H=16.0f;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_entity_Entity g_Entity_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<3>";
		bb_std_lang.popErr();
		return this;
	}
	public bool f_Active=false;
	public virtual void m_Activate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<48>";
		f_Active=true;
		bb_std_lang.popErr();
	}
	public float f_X=.0f;
	public float f_Y=.0f;
	public virtual void m_SetPosition(float t_tX,float t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<30>";
		f_X=t_tX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<31>";
		f_Y=t_tY;
		bb_std_lang.popErr();
	}
	public float f_XS=.0f;
	public float f_YS=.0f;
	public float f_Z=.0f;
	public bool f_onFloor=false;
	public virtual void m_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<35>";
		if(f_Z>=0.0f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<36>";
			f_onFloor=true;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<37>";
			f_Z=0.0f;
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<39>";
			f_onFloor=false;
		}
		bb_std_lang.popErr();
	}
	public virtual bool m_IsOnScreen(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<56>";
		bool t_=bb_functions.bb_functions_RectOverRect(f_X,f_Y,f_W,f_H,(float)(bb__LDApp.g_ScreenX-50),(float)(bb__LDApp.g_ScreenY-50),(float)(bb__LDApp.g_ScreenWidth+100),(float)(bb__LDApp.g_ScreenHeight+100));
		bb_std_lang.popErr();
		return t_;
	}
	public virtual void m_Deactivate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/entity.monkey<52>";
		f_Active=false;
		bb_std_lang.popErr();
	}
	public float f_ZS=.0f;
	public virtual void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
}
class bb_dog_Dog : bb_entity_Entity{
	public static bb_dog_Dog[] g_a;
	public virtual bb_dog_Dog g_Dog_new(bb_level_Level t_tLev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<54>";
		base.g_Entity_new2();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<55>";
		f_level=t_tLev;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_dog_Dog g_Dog_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<3>";
		base.g_Entity_new2();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<3>";
		bb_std_lang.popErr();
		return this;
	}
	public static bb_graphics_Image g_gfxStandFront;
	public static void g_Init(bb_level_Level t_tLev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<12>";
		g_a=new bb_dog_Dog[10];
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<13>";
		for(int t_i=0;t_i<10;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<14>";
			g_a[t_i]=(new bb_dog_Dog()).g_Dog_new(t_tLev);
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<17>";
		g_gfxStandFront=bb_gfx_GFX.g_Tileset.m_GrabImage(0,80,16,16,1,1);
		bb_std_lang.popErr();
	}
	public static int g_NextDog;
	public override void m_Activate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<59>";
		base.m_Activate();
		bb_std_lang.popErr();
	}
	public static int g_Create(float t_tX,float t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<41>";
		int t_tDog=g_NextDog;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<43>";
		g_a[g_NextDog].m_Activate();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<44>";
		g_a[g_NextDog].m_SetPosition(t_tX,t_tY);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<46>";
		g_NextDog+=1;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<47>";
		if(g_NextDog==10){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<48>";
			g_NextDog=0;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<51>";
		bb_std_lang.popErr();
		return t_tDog;
	}
	public override void m_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<64>";
		if(!m_IsOnScreen()){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<65>";
			m_Deactivate();
		}
		bb_std_lang.popErr();
	}
	public static void g_UpdateAll(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<22>";
		for(int t_i=0;t_i<10;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<23>";
			if(g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<24>";
				g_a[t_i].m_Update();
			}
		}
		bb_std_lang.popErr();
	}
	public override void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<71>";
		bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y,0,true);
		bb_std_lang.popErr();
	}
	public static void g_RenderAll(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<30>";
		for(int t_i=0;t_i<10;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<31>";
			if(g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<32>";
				if(g_a[t_i].m_IsOnScreen()){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/dog.monkey<33>";
					g_a[t_i].m_Render();
				}
			}
		}
		bb_std_lang.popErr();
	}
}
class bb_yeti_Yeti : bb_entity_Entity{
	public static bb_yeti_Yeti[] g_a;
	public virtual bb_yeti_Yeti g_Yeti_new(bb_level_Level t_tLev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<69>";
		base.g_Entity_new2();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<70>";
		f_level=t_tLev;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_yeti_Yeti g_Yeti_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<3>";
		base.g_Entity_new2();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<3>";
		bb_std_lang.popErr();
		return this;
	}
	public static bb_graphics_Image g_gfxStandFront;
	public static bb_graphics_Image g_gfxRunFront;
	public static bb_graphics_Image g_gfxRunLeft;
	public static bb_graphics_Image g_gfxRunRight;
	public static void g_Init(bb_level_Level t_tLev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<15>";
		g_a=new bb_yeti_Yeti[1];
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<16>";
		for(int t_i=0;t_i<1;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<17>";
			g_a[t_i]=(new bb_yeti_Yeti()).g_Yeti_new(t_tLev);
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<20>";
		g_gfxStandFront=bb_gfx_GFX.g_Tileset.m_GrabImage(48,0,22,32,2,1);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<21>";
		g_gfxRunFront=bb_gfx_GFX.g_Tileset.m_GrabImage(48,32,22,32,2,1);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<22>";
		g_gfxRunLeft=bb_gfx_GFX.g_Tileset.m_GrabImage(48,64,22,32,2,1);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<23>";
		g_gfxRunRight=bb_gfx_GFX.g_Tileset.m_GrabImage(48,96,22,32,2,1);
		bb_std_lang.popErr();
	}
	public static int g_NextYeti;
	public int f_Status=0;
	public override void m_Activate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<74>";
		base.m_Activate();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<75>";
		f_Status=0;
		bb_std_lang.popErr();
	}
	public static int g_Create(float t_tX,float t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<47>";
		int t_thisYeti=g_NextYeti;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<48>";
		g_a[t_thisYeti].m_Activate();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<49>";
		g_NextYeti+=1;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<50>";
		if(g_NextYeti==1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<51>";
			g_NextYeti=0;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<53>";
		bb_std_lang.popErr();
		return t_thisYeti;
	}
	public virtual void m_StartWaiting(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<120>";
		f_Status=0;
		bb_std_lang.popErr();
	}
	public float f_aniRunFrameTimer=0.0f;
	public int f_aniRunFrame=0;
	public float f_aniWaitFrameTimer=0.0f;
	public int f_aniWaitFrame=0;
	public int f_D=0;
	public virtual void m_UpdateControlled(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<217>";
		if(bb_controls_Controls.g_LeftHit){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<218>";
			if(f_D>0){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<219>";
				f_D-=1;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<221>";
				if(f_onFloor){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<222>";
					f_XS=-0.2f;
				}
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<227>";
		if(bb_controls_Controls.g_RightHit){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<228>";
			if(f_D<6){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<229>";
				f_D+=1;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<231>";
				if(f_onFloor){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<232>";
					f_XS=0.2f;
				}
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<237>";
		if(bb_controls_Controls.g_DownHit){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<238>";
			f_D=3;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<241>";
		if(bb_controls_Controls.g_UpHit){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<242>";
			if(f_onFloor){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<243>";
				f_ZS=-1.5f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<244>";
				f_XS*=1.5f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<245>";
				f_YS*=1.5f;
			}
		}
		bb_std_lang.popErr();
	}
	public float f_MaxYS=.0f;
	public float f_TargetXS=.0f;
	public virtual void m_UpdateChasing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<141>";
		m_UpdateControlled();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<143>";
		if(f_onFloor){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<144>";
			int t_=f_D;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<145>";
			if(t_==0){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<146>";
				f_MaxYS=0.0f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<147>";
				f_TargetXS=0.0f;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<148>";
				if(t_==1){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<149>";
					f_MaxYS=1.0f;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<150>";
					f_TargetXS=-2.0f;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<151>";
					if(t_==2){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<152>";
						f_MaxYS=2.0f;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<153>";
						f_TargetXS=-1.0f;
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<154>";
						if(t_==3){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<155>";
							f_MaxYS=3.0f;
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<156>";
							f_TargetXS=0.0f;
						}else{
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<157>";
							if(t_==4){
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<158>";
								f_MaxYS=2.0f;
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<159>";
								f_TargetXS=1.0f;
							}else{
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<160>";
								if(t_==5){
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<161>";
									f_MaxYS=1.0f;
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<162>";
									f_TargetXS=2.0f;
								}else{
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<163>";
									if(t_==6){
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<164>";
										f_MaxYS=0.0f;
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<165>";
										f_TargetXS=0.0f;
									}
								}
							}
						}
					}
				}
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<168>";
			if(f_YS>f_MaxYS){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<169>";
				f_YS*=1.0f-0.05f*bb__LDApp.g_Delta;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<171>";
				f_YS+=0.05f*bb__LDApp.g_Delta;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<174>";
			if(f_XS>f_TargetXS-0.05f*bb__LDApp.g_Delta && f_XS<f_TargetXS+0.05f*bb__LDApp.g_Delta){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<175>";
				f_XS=f_TargetXS;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<176>";
				if(f_XS<f_TargetXS){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<177>";
					f_XS+=0.05f*bb__LDApp.g_Delta;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<178>";
					if(f_XS>f_TargetXS){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<179>";
						f_XS-=0.05f*bb__LDApp.g_Delta;
					}
				}
			}
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<182>";
			f_ZS+=0.05f*bb__LDApp.g_Delta;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<185>";
		f_X+=f_XS*bb__LDApp.g_Delta;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<186>";
		f_Y+=f_YS*bb__LDApp.g_Delta;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<187>";
		f_Z+=f_ZS*bb__LDApp.g_Delta;
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateDazed(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateEating(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateWaitingHappy(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_StartChasing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<130>";
		f_D=3;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<131>";
		f_XS=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<132>";
		f_YS=0.5f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<133>";
		f_ZS=-1.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<134>";
		f_Z=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<135>";
		f_Status=1;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<136>";
		bb_sfx_SFX.g_Music("chase",1);
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateWaiting(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<124>";
		if(bb_controls_Controls.g_DownHit){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<125>";
			m_StartChasing();
		}
		bb_std_lang.popErr();
	}
	public override void m_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<80>";
		base.m_Update();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<82>";
		f_aniRunFrameTimer+=1.0f*bb__LDApp.g_Delta;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<83>";
		if(f_aniRunFrameTimer>=5.0f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<84>";
			f_aniRunFrameTimer=0.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<85>";
			f_aniRunFrame+=1;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<86>";
			if(f_aniRunFrame>=2){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<87>";
				f_aniRunFrame=0;
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<91>";
		f_aniWaitFrameTimer+=1.0f*bb__LDApp.g_Delta;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<92>";
		if(f_aniWaitFrameTimer>=30.0f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<93>";
			f_aniWaitFrameTimer=0.0f;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<94>";
			f_aniWaitFrame+=1;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<95>";
			if(f_aniWaitFrame>=2){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<96>";
				f_aniWaitFrame=0;
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<100>";
		int t_=f_Status;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<101>";
		if(t_==1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<102>";
			m_UpdateChasing();
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<103>";
			if(t_==3){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<104>";
				m_UpdateDazed();
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<105>";
				if(t_==2){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<106>";
					m_UpdateEating();
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<107>";
					if(t_==4){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<108>";
						m_UpdateWaitingHappy();
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<109>";
						if(t_==0){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<110>";
							m_UpdateWaiting();
						}
					}
				}
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<113>";
		if(!m_IsOnScreen()){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<114>";
			m_Deactivate();
		}
		bb_std_lang.popErr();
	}
	public static void g_UpdateAll(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<28>";
		for(int t_i=0;t_i<1;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<29>";
			if(g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<30>";
				g_a[t_i].m_Update();
			}
		}
		bb_std_lang.popErr();
	}
	public virtual void m_RenderChasing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<274>";
		int t_=f_D;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<275>";
		if(t_==0){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<276>";
			bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y+f_Z,f_aniWaitFrame,true);
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<277>";
			if(t_==1 || t_==2){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<278>";
				bb_gfx_GFX.g_Draw(g_gfxRunLeft,f_X,f_Y+f_Z,f_aniRunFrame,true);
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<279>";
				if(t_==3){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<280>";
					bb_gfx_GFX.g_Draw(g_gfxRunFront,f_X,f_Y+f_Z,f_aniRunFrame,true);
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<281>";
					if(t_==4 || t_==5){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<282>";
						bb_gfx_GFX.g_Draw(g_gfxRunRight,f_X,f_Y+f_Z,f_aniRunFrame,true);
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<283>";
						if(t_==6){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<284>";
							bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y+f_Z,f_aniWaitFrame,true);
						}
					}
				}
			}
		}
		bb_std_lang.popErr();
	}
	public virtual void m_RenderDazed(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_RenderEating(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_RenderWaitingHappy(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_RenderWaiting(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<270>";
		bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y,f_aniWaitFrame,true);
		bb_std_lang.popErr();
	}
	public override void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<253>";
		int t_=f_Status;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<254>";
		if(t_==1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<255>";
			m_RenderChasing();
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<256>";
			if(t_==3){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<257>";
				m_RenderDazed();
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<258>";
				if(t_==2){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<259>";
					m_RenderEating();
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<260>";
					if(t_==4){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<261>";
						m_RenderWaitingHappy();
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<262>";
						if(t_==0){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<263>";
							m_RenderWaiting();
						}else{
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<264>";
							if(t_==5){
							}
						}
					}
				}
			}
		}
		bb_std_lang.popErr();
	}
	public static void g_RenderAll(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<36>";
		for(int t_i=0;t_i<1;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<37>";
			if(g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<38>";
				if(g_a[t_i].m_IsOnScreen()){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/yeti.monkey<39>";
					g_a[t_i].m_Render();
				}
			}
		}
		bb_std_lang.popErr();
	}
}
class bb_skier_Skier : bb_entity_Entity{
	public static bb_skier_Skier[] g_a;
	public virtual bb_skier_Skier g_Skier_new(bb_level_Level t_tLev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<81>";
		base.g_Entity_new2();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<82>";
		f_level=t_tLev;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_skier_Skier g_Skier_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<3>";
		base.g_Entity_new2();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<3>";
		bb_std_lang.popErr();
		return this;
	}
	public static bb_graphics_Image g_gfxSkiL;
	public static bb_graphics_Image g_gfxSkiLLD;
	public static bb_graphics_Image g_gfxSkiLDD;
	public static bb_graphics_Image g_gfxSkiD;
	public static bb_graphics_Image g_gfxSkiRDD;
	public static bb_graphics_Image g_gfxSkiRRD;
	public static bb_graphics_Image g_gfxSkiR;
	public static void g_Init(bb_level_Level t_tLev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<20>";
		g_a=new bb_skier_Skier[20];
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<21>";
		for(int t_i=0;t_i<20;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<22>";
			g_a[t_i]=(new bb_skier_Skier()).g_Skier_new(t_tLev);
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<25>";
		g_gfxSkiL=bb_gfx_GFX.g_Tileset.m_GrabImage(70,128,22,32,1,1);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<26>";
		g_gfxSkiLLD=g_gfxSkiL;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<27>";
		g_gfxSkiLDD=g_gfxSkiL;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<28>";
		g_gfxSkiD=g_gfxSkiL;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<29>";
		g_gfxSkiRDD=g_gfxSkiL;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<30>";
		g_gfxSkiRRD=g_gfxSkiL;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<31>";
		g_gfxSkiR=g_gfxSkiL;
		bb_std_lang.popErr();
	}
	public static int g_NextSkier;
	public int f_TargetYeti=0;
	public override void m_Activate(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<242>";
		base.m_Activate();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<243>";
		f_TargetYeti=-1;
		bb_std_lang.popErr();
	}
	public static int g_Create(float t_tX,float t_tY){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<58>";
		int t_tI=g_NextSkier;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<59>";
		g_a[t_tI].m_Activate();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<60>";
		g_a[t_tI].m_SetPosition(t_tX,t_tY);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<62>";
		g_NextSkier+=1;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<63>";
		if(g_NextSkier>=20){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<64>";
			g_NextSkier=0;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<67>";
		bb_std_lang.popErr();
		return t_tI;
	}
	public int f_Status=0;
	public int f_D=0;
	public virtual int m_FindNearestYeti(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<247>";
		int t_nIndex=-1;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<248>";
		float t_nDistance=99999999.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<250>";
		for(int t_i=0;t_i<1;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<251>";
			if(bb_yeti_Yeti.g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<252>";
				if(bb_yeti_Yeti.g_a[t_i].f_Y>f_Y){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<253>";
					float t_tDist=bb_yeti_Yeti.g_a[t_i].f_Y-f_Y;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<254>";
					if(t_tDist<200.0f){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<255>";
						if(t_tDist<t_nDistance){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<256>";
							t_nIndex=t_i;
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<257>";
							t_nDistance=t_tDist;
						}
					}
				}
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<264>";
		bb_std_lang.popErr();
		return t_nIndex;
	}
	public virtual void m_StartTeasing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<208>";
		f_Status=4;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<209>";
		f_D=3;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<210>";
		f_XS=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<211>";
		f_YS=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<212>";
		f_TargetYeti=m_FindNearestYeti();
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateDazed(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateDead(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateFalling(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
	}
	public float f_MaxYS=.0f;
	public float f_TargetXS=.0f;
	public virtual void m_UpdateSkiing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<118>";
		f_D=bb_math.bb_math_Clamp(f_D,0,6);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<120>";
		if(bb_random.bb_random_Rnd()<0.02f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<121>";
			if(bb_random.bb_random_Rnd()<0.2f){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<122>";
				f_D=3;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<124>";
				if(bb_random.bb_random_Rnd()<0.5f){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<125>";
					f_D-=1;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<127>";
					f_D+=1;
				}
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<133>";
		if(f_onFloor){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<134>";
			int t_=f_D;
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<135>";
			if(t_==0){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<136>";
				f_MaxYS=0.0f;
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<137>";
				f_TargetXS=0.0f;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<138>";
				if(t_==1){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<139>";
					f_MaxYS=1.0f;
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<140>";
					f_TargetXS=-2.0f;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<141>";
					if(t_==2){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<142>";
						f_MaxYS=2.0f;
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<143>";
						f_TargetXS=-1.0f;
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<144>";
						if(t_==3){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<145>";
							f_MaxYS=3.0f;
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<146>";
							f_TargetXS=0.0f;
						}else{
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<147>";
							if(t_==4){
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<148>";
								f_MaxYS=2.0f;
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<149>";
								f_TargetXS=1.0f;
							}else{
								bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<150>";
								if(t_==5){
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<151>";
									f_MaxYS=1.0f;
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<152>";
									f_TargetXS=2.0f;
								}else{
									bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<153>";
									if(t_==6){
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<154>";
										f_MaxYS=0.0f;
										bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<155>";
										f_TargetXS=0.0f;
									}
								}
							}
						}
					}
				}
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<158>";
			if(f_YS>f_MaxYS){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<159>";
				f_YS*=1.0f-0.05f*bb__LDApp.g_Delta;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<161>";
				f_YS*=1.0f+0.05f*bb__LDApp.g_Delta;
			}
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<164>";
			if(f_XS>f_TargetXS-0.05f*bb__LDApp.g_Delta && f_XS<f_TargetXS+0.05f*bb__LDApp.g_Delta){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<165>";
				f_XS=f_TargetXS;
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<166>";
				if(f_XS<f_TargetXS){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<167>";
					f_XS+=0.05f*bb__LDApp.g_Delta;
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<168>";
					if(f_XS>f_TargetXS){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<169>";
						f_XS-=0.05f*bb__LDApp.g_Delta;
					}
				}
			}
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<173>";
			f_ZS+=0.05f*bb__LDApp.g_Delta;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<176>";
		f_X+=f_XS*bb__LDApp.g_Delta;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<177>";
		f_Y+=f_YS*bb__LDApp.g_Delta;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<178>";
		f_Z+=f_ZS*bb__LDApp.g_Delta;
		bb_std_lang.popErr();
	}
	public virtual void m_StartSkiing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<107>";
		f_Status=0;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<108>";
		f_Z=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<109>";
		f_D=3;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<110>";
		f_YS=3.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<111>";
		f_XS=0.0f;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<112>";
		f_ZS=-2.0f;
		bb_std_lang.popErr();
	}
	public virtual void m_UpdateTeasing(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<216>";
		if(bb_random.bb_random_Rnd()<0.05f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<217>";
			f_Y=f_Y+1.0f;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<220>";
		if(bb_yeti_Yeti.g_a[f_TargetYeti].f_Y-f_Y<2.0f){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<221>";
			m_StartSkiing();
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<222>";
			if(bb_yeti_Yeti.g_a[f_TargetYeti].f_Y-f_Y<50.0f){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<223>";
				if(bb_random.bb_random_Rnd()<0.02f){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<224>";
					m_StartSkiing();
				}
			}
		}
		bb_std_lang.popErr();
	}
	public override void m_Update(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<87>";
		int t_=f_Status;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<88>";
		if(t_==2){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<89>";
			m_UpdateDazed();
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<90>";
			if(t_==3){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<91>";
				m_UpdateDead();
			}else{
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<92>";
				if(t_==1){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<93>";
					m_UpdateFalling();
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<94>";
					if(t_==0){
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<95>";
						m_UpdateSkiing();
					}else{
						bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<96>";
						if(t_==4){
							bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<97>";
							m_UpdateTeasing();
						}
					}
				}
			}
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<100>";
		if(!m_IsOnScreen()){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<101>";
			m_Deactivate();
		}
		bb_std_lang.popErr();
	}
	public static void g_UpdateAll(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<35>";
		for(int t_i=0;t_i<20;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<36>";
			if(g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<37>";
				if(g_a[t_i].m_IsOnScreen()){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<38>";
					g_a[t_i].m_Update();
				}
			}
		}
		bb_std_lang.popErr();
	}
	public override void m_Render(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<230>";
		bb_gfx_GFX.g_Draw(g_gfxSkiL,f_X,f_Y+f_Z,0,true);
		bb_std_lang.popErr();
	}
	public virtual void m_RenderMarker(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<234>";
		if(f_X<(float)(bb__LDApp.g_ScreenX)){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<235>";
			bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,0.0f,300.0f,0,176,8,9,0);
		}else{
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<236>";
			if(f_X>(float)(bb__LDApp.g_ScreenX+bb__LDApp.g_ScreenWidth)){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<237>";
				bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,232.0f,300.0f,8,176,8,9,0);
			}
		}
		bb_std_lang.popErr();
	}
	public static void g_RenderAll(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<45>";
		for(int t_i=0;t_i<20;t_i=t_i+1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<46>";
			if(g_a[t_i].f_Active==true){
				bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<47>";
				if(g_a[t_i].m_IsOnScreen()){
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<48>";
					g_a[t_i].m_Render();
				}else{
					bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/game/skier.monkey<50>";
					g_a[t_i].m_RenderMarker();
				}
			}
		}
		bb_std_lang.popErr();
	}
}
class bb_yeti_YetiStatusTypes : Object{
}
class bb_skier_SkierStatusType : Object{
}
class bb_entity_EntityMoveDirection : Object{
}
class bb_raztext_RazChar : Object{
	public virtual bb_raztext_RazChar g_RazChar_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/raztext.monkey<483>";
		bb_std_lang.popErr();
		return this;
	}
	public int f_XOff=0;
	public int f_YOff=0;
	public String f_TextValue="";
	public bool f_Visible=true;
}
class bb_list_List : Object{
	public virtual bb_list_List g_List_new(){
		bb_std_lang.pushErr();
		bb_std_lang.popErr();
		return this;
	}
	public bb_list_Node f__head=((new bb_list_HeadNode()).g_HeadNode_new());
	public virtual bb_list_Node m_AddLast(bb_raztext_RazChar[] t_data){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<120>";
		bb_list_Node t_=(new bb_list_Node()).g_Node_new(f__head,f__head.f__pred,t_data);
		bb_std_lang.popErr();
		return t_;
	}
	public virtual bb_list_List g_List_new2(bb_raztext_RazChar[][] t_data){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<13>";
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<13>";
		bb_raztext_RazChar[][] t_=t_data;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<13>";
		int t_2=0;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<13>";
		while(t_2<bb_std_lang.length(t_)){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<13>";
			bb_raztext_RazChar[] t_t=t_[t_2];
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<13>";
			t_2=t_2+1;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<14>";
			m_AddLast(t_t);
		}
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_list_Enumerator m_ObjectEnumerator(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<124>";
		bb_list_Enumerator t_=(new bb_list_Enumerator()).g_Enumerator_new(this);
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_list_Node : Object{
	public bb_list_Node f__succ=null;
	public bb_list_Node f__pred=null;
	public bb_raztext_RazChar[] f__data=new bb_raztext_RazChar[0];
	public virtual bb_list_Node g_Node_new(bb_list_Node t_succ,bb_list_Node t_pred,bb_raztext_RazChar[] t_data){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<199>";
		f__succ=t_succ;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<200>";
		f__pred=t_pred;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<201>";
		f__succ.f__pred=this;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<202>";
		f__pred.f__succ=this;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<203>";
		f__data=t_data;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_list_Node g_Node_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<196>";
		bb_std_lang.popErr();
		return this;
	}
}
class bb_list_HeadNode : bb_list_Node{
	public virtual bb_list_HeadNode g_HeadNode_new(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<248>";
		base.g_Node_new2();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<249>";
		f__succ=(this);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<250>";
		f__pred=(this);
		bb_std_lang.popErr();
		return this;
	}
}
class bb_list_Enumerator : Object{
	public bb_list_List f__list=null;
	public bb_list_Node f__curr=null;
	public virtual bb_list_Enumerator g_Enumerator_new(bb_list_List t_list){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<264>";
		f__list=t_list;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<265>";
		f__curr=t_list.f__head.f__succ;
		bb_std_lang.popErr();
		return this;
	}
	public virtual bb_list_Enumerator g_Enumerator_new2(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<261>";
		bb_std_lang.popErr();
		return this;
	}
	public virtual bool m_HasNext(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<269>";
		while(f__curr.f__succ.f__pred!=f__curr){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<270>";
			f__curr=f__curr.f__succ;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<272>";
		bool t_=f__curr!=f__list.f__head;
		bb_std_lang.popErr();
		return t_;
	}
	public virtual bb_raztext_RazChar[] m_NextObject(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<276>";
		bb_raztext_RazChar[] t_data=f__curr.f__data;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<277>";
		f__curr=f__curr.f__succ;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/list.monkey<278>";
		bb_std_lang.popErr();
		return t_data;
	}
}
class bb_{
	public static int bbMain(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<150>";
		(new bb__LDApp()).g_LDApp_new();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/ld.monkey<151>";
		bb_std_lang.popErr();
		return 0;
	}
	public static int bbInit(){
		bb_graphics.bb_graphics_device=null;
		bb_input.bb_input_device=null;
		bb_audio.bb_audio_device=null;
		bb_app.bb_app_device=null;
		bb_graphics.bb_graphics_context=(new bb_graphics_GraphicsContext()).g_GraphicsContext_new();
		bb_graphics_Image.g_DefaultFlags=0;
		bb_graphics.bb_graphics_renderDevice=null;
		bb_gfx_GFX.g_Tileset=null;
		bb_screenmanager_ScreenManager.g_Screens=null;
		bb_screenmanager_ScreenManager.g_FadeAlpha=.0f;
		bb_screenmanager_ScreenManager.g_FadeRate=.0f;
		bb_screenmanager_ScreenManager.g_FadeRed=.0f;
		bb_screenmanager_ScreenManager.g_FadeGreen=.0f;
		bb_screenmanager_ScreenManager.g_FadeBlue=.0f;
		bb_screenmanager_ScreenManager.g_FadeMode=0;
		bb_sfx_SFX.g_ActiveChannel=0;
		bb_sfx_SFX.g_Sounds=null;
		bb_sfx_SFX.g_Musics=null;
		bb_controls_Controls.g_TCMove=null;
		bb__LDApp.g_ScreenHeight=360;
		bb__LDApp.g_ScreenWidth=240;
		bb_controls_Controls.g_TCAction1=null;
		bb_controls_Controls.g_TCAction2=null;
		bb_controls_Controls.g_TCEscapeKey=null;
		bb_controls_Controls.g_TCButtons=new bb_touchbutton_TouchButton[0];
		bb_raztext_RazText.g_TextSheet=null;
		bb_screenmanager_ScreenManager.g_gameScreen=null;
		bb_screenmanager_ScreenManager.g_ActiveScreen=null;
		bb_screenmanager_ScreenManager.g_ActiveScreenName="";
		bb_controls_Controls.g_ControlMethod=0;
		bb__LDApp.g_RefreshRate=0;
		bb__LDApp.g_Delta=.0f;
		bb_autofit_VirtualDisplay.g_Display=null;
		bb_controls_Controls.g_LeftHit=false;
		bb_controls_Controls.g_RightHit=false;
		bb_controls_Controls.g_DownHit=false;
		bb_controls_Controls.g_UpHit=false;
		bb_controls_Controls.g_ActionHit=false;
		bb_controls_Controls.g_Action2Hit=false;
		bb_controls_Controls.g_EscapeHit=false;
		bb_controls_Controls.g_LeftKey=65;
		bb_controls_Controls.g_LeftDown=false;
		bb_controls_Controls.g_RightKey=68;
		bb_controls_Controls.g_RightDown=false;
		bb_controls_Controls.g_UpKey=87;
		bb_controls_Controls.g_UpDown=false;
		bb_controls_Controls.g_DownKey=83;
		bb_controls_Controls.g_DownDown=false;
		bb_controls_Controls.g_ActionKey=32;
		bb_controls_Controls.g_ActionDown=false;
		bb_controls_Controls.g_Action2Key=13;
		bb_controls_Controls.g_Action2Down=false;
		bb_controls_Controls.g_EscapeKey=27;
		bb_controls_Controls.g_EscapeDown=false;
		bb_controls_Controls.g_TouchPoint=false;
		bb__LDApp.g_TargetScreenX=0.0f;
		bb__LDApp.g_ActualScreenX=0;
		bb__LDApp.g_ScreenMoveRate=0.4f;
		bb__LDApp.g_TargetScreenY=0.0f;
		bb__LDApp.g_ActualScreenY=0;
		bb__LDApp.g_ScreenX=0;
		bb__LDApp.g_ScreenY=0;
		bb_screenmanager_ScreenManager.g_NextScreenName="";
		bb__LDApp.g_level=null;
		bb_dog_Dog.g_a=new bb_dog_Dog[0];
		bb_dog_Dog.g_gfxStandFront=null;
		bb_yeti_Yeti.g_a=new bb_yeti_Yeti[0];
		bb_yeti_Yeti.g_gfxStandFront=null;
		bb_yeti_Yeti.g_gfxRunFront=null;
		bb_yeti_Yeti.g_gfxRunLeft=null;
		bb_yeti_Yeti.g_gfxRunRight=null;
		bb_skier_Skier.g_a=new bb_skier_Skier[0];
		bb_skier_Skier.g_gfxSkiL=null;
		bb_skier_Skier.g_gfxSkiLLD=null;
		bb_skier_Skier.g_gfxSkiLDD=null;
		bb_skier_Skier.g_gfxSkiD=null;
		bb_skier_Skier.g_gfxSkiRDD=null;
		bb_skier_Skier.g_gfxSkiRRD=null;
		bb_skier_Skier.g_gfxSkiR=null;
		bb_yeti_Yeti.g_NextYeti=0;
		bb_dog_Dog.g_NextDog=0;
		bb_skier_Skier.g_NextSkier=0;
		bb_sfx_SFX.g_MusicActive=true;
		bb_sfx_SFX.g_CurrentMusic="";
		bb_random.bb_random_Seed=1234;
		return 0;
	}
}
class bb_autofit{
	public static int bb_autofit_SetVirtualDisplay(int t_width,int t_height,float t_zoom){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<110>";
		(new bb_autofit_VirtualDisplay()).g_VirtualDisplay_new(t_width,t_height,t_zoom);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<111>";
		bb_std_lang.popErr();
		return 0;
	}
	public static float bb_autofit_VDeviceWidth(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<206>";
		bb_std_lang.popErr();
		return bb_autofit_VirtualDisplay.g_Display.f_vwidth;
	}
	public static float bb_autofit_VMouseX(bool t_limit){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<188>";
		float t_=bb_autofit_VirtualDisplay.g_Display.m_VMouseX(t_limit);
		bb_std_lang.popErr();
		return t_;
	}
	public static float bb_autofit_VDeviceHeight(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<210>";
		bb_std_lang.popErr();
		return bb_autofit_VirtualDisplay.g_Display.f_vheight;
	}
	public static float bb_autofit_VMouseY(bool t_limit){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<192>";
		float t_=bb_autofit_VirtualDisplay.g_Display.m_VMouseY(t_limit);
		bb_std_lang.popErr();
		return t_;
	}
	public static int bb_autofit_UpdateVirtualDisplay(bool t_zoomborders,bool t_keepborders){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<171>";
		bb_autofit_VirtualDisplay.g_Display.m_UpdateVirtualDisplay(t_zoomborders,t_keepborders);
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/autofit.monkey<172>";
		bb_std_lang.popErr();
		return 0;
	}
}
class bb_controls{
}
class bb_functions{
	public static bool bb_functions_PointInRect(float t_X,float t_Y,float t_X1,float t_Y1,float t_W,float t_H){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<12>";
		if(t_X<t_X1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<12>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<13>";
		if(t_Y<t_Y1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<13>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<14>";
		if(t_X>t_X1+t_W){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<14>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<15>";
		if(t_Y>t_Y1+t_H){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<15>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<16>";
		bb_std_lang.popErr();
		return true;
	}
	public static bool bb_functions_RectOverRect(float t_tX1,float t_tY1,float t_tW1,float t_tH1,float t_tX2,float t_tY2,float t_tW2,float t_tH2){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<4>";
		if(t_tX2>t_tX1+t_tW1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<4>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<5>";
		if(t_tX1>t_tX2+t_tW2){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<5>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<6>";
		if(t_tY2>t_tY1+t_tH1){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<6>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<7>";
		if(t_tY1>t_tY2+t_tH2){
			bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<7>";
			bb_std_lang.popErr();
			return false;
		}
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/functions.monkey<8>";
		bb_std_lang.popErr();
		return true;
	}
}
class bb_gfx{
	public static void bb_gfx_DrawHollowRect(int t_tX,int t_tY,int t_tW,int t_tH){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<42>";
		int t_X1=t_tX;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<43>";
		int t_Y1=t_tY;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<44>";
		int t_X2=t_tX+t_tW;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<45>";
		int t_Y2=t_tY+t_tH;
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<47>";
		bb_graphics.bb_graphics_DrawLine((float)(t_X1),(float)(t_Y1),(float)(t_X2),(float)(t_Y1));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<48>";
		bb_graphics.bb_graphics_DrawLine((float)(t_X1),(float)(t_Y2),(float)(t_X2),(float)(t_Y2));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<49>";
		bb_graphics.bb_graphics_DrawLine((float)(t_X1),(float)(t_Y1),(float)(t_X1),(float)(t_Y2));
		bb_std_lang.errInfo="C:/Users/Chris/Documents/GitHub/CM-YATY/src/framework/gfx.monkey<50>";
		bb_graphics.bb_graphics_DrawLine((float)(t_X2),(float)(t_Y1),(float)(t_X2),(float)(t_Y2));
		bb_std_lang.popErr();
	}
}
class bb_raztext{
}
class bb_rect{
}
class bb_screen{
}
class bb_screenmanager{
}
class bb_sfx{
}
class bb_touchbutton{
}
class bb_virtualstick{
}
class bb_bump{
}
class bb_dog{
}
class bb_entity{
}
class bb_flag{
}
class bb_jump{
}
class bb_level{
}
class bb_rock{
}
class bb_skier{
}
class bb_snowboarder{
}
class bb_tree{
}
class bb_yeti{
}
class bb_creditsscreen{
}
class bb_gamescreen{
}
class bb_logoscreen{
}
class bb_optionsscreen{
}
class bb_postgamescreen{
}
class bb_pregamescreen{
}
class bb_titlescreen{
}
class bb_waitjoypadscreen{
}
class bb_asyncevent{
}
class bb_databuffer{
}
class bb_thread{
}
class bb_app{
	public static bb_app_AppDevice bb_app_device;
	public static int bb_app_SetUpdateRate(int t_hertz){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<145>";
		int t_=bb_app.bb_app_device.SetUpdateRate(t_hertz);
		bb_std_lang.popErr();
		return t_;
	}
	public static String bb_app_LoadString(String t_path){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/app.monkey<141>";
		String t_=bb_app.bb_app_device.LoadString(bb_data.bb_data_FixDataPath(t_path));
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_asyncimageloader{
}
class bb_asyncloaders{
}
class bb_asyncsoundloader{
}
class bb_audio{
	public static gxtkAudio bb_audio_device;
	public static int bb_audio_SetAudioDevice(gxtkAudio t_dev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/audio.monkey<17>";
		bb_audio.bb_audio_device=t_dev;
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_audio_MusicState(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/audio.monkey<95>";
		int t_=bb_audio.bb_audio_device.MusicState();
		bb_std_lang.popErr();
		return t_;
	}
	public static int bb_audio_PlayMusic(String t_path,int t_flags){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/audio.monkey<79>";
		int t_=bb_audio.bb_audio_device.PlayMusic(bb_data.bb_data_FixDataPath(t_path),t_flags);
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_audiodevice{
}
class bb_data{
	public static String bb_data_FixDataPath(String t_path){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/data.monkey<3>";
		int t_i=t_path.IndexOf(":/",0);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/data.monkey<4>";
		if(t_i!=-1 && t_path.IndexOf("/",0)==t_i+1){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/data.monkey<4>";
			bb_std_lang.popErr();
			return t_path;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/data.monkey<5>";
		if(t_path.StartsWith("./") || t_path.StartsWith("/")){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/data.monkey<5>";
			bb_std_lang.popErr();
			return t_path;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/data.monkey<6>";
		String t_="monkey://data/"+t_path;
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_graphics{
	public static gxtkGraphics bb_graphics_device;
	public static int bb_graphics_SetGraphicsDevice(gxtkGraphics t_dev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<58>";
		bb_graphics.bb_graphics_device=t_dev;
		bb_std_lang.popErr();
		return 0;
	}
	public static bb_graphics_GraphicsContext bb_graphics_context;
	public static bb_graphics_Image bb_graphics_LoadImage(String t_path,int t_frameCount,int t_flags){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<229>";
		gxtkSurface t_surf=bb_graphics.bb_graphics_device.LoadSurface(bb_data.bb_data_FixDataPath(t_path));
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<230>";
		if((t_surf)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<230>";
			bb_graphics_Image t_=((new bb_graphics_Image()).g_Image_new()).m_Init(t_surf,t_frameCount,t_flags);
			bb_std_lang.popErr();
			return t_;
		}
		bb_std_lang.popErr();
		return null;
	}
	public static bb_graphics_Image bb_graphics_LoadImage2(String t_path,int t_frameWidth,int t_frameHeight,int t_frameCount,int t_flags){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<234>";
		bb_graphics_Image t_atlas=bb_graphics.bb_graphics_LoadImage(t_path,1,0);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<235>";
		if((t_atlas)!=null){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<235>";
			bb_graphics_Image t_=t_atlas.m_GrabImage(0,0,t_frameWidth,t_frameHeight,t_frameCount,t_flags);
			bb_std_lang.popErr();
			return t_;
		}
		bb_std_lang.popErr();
		return null;
	}
	public static int bb_graphics_SetFont(bb_graphics_Image t_font,int t_firstChar){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<524>";
		if(!((t_font)!=null)){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<525>";
			if(!((bb_graphics.bb_graphics_context.f_defaultFont)!=null)){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<526>";
				bb_graphics.bb_graphics_context.f_defaultFont=bb_graphics.bb_graphics_LoadImage("mojo_font.png",96,2);
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<528>";
			t_font=bb_graphics.bb_graphics_context.f_defaultFont;
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<529>";
			t_firstChar=32;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<531>";
		bb_graphics.bb_graphics_context.f_font=t_font;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<532>";
		bb_graphics.bb_graphics_context.f_firstChar=t_firstChar;
		bb_std_lang.popErr();
		return 0;
	}
	public static gxtkGraphics bb_graphics_renderDevice;
	public static int bb_graphics_SetMatrix(float t_ix,float t_iy,float t_jx,float t_jy,float t_tx,float t_ty){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<289>";
		bb_graphics.bb_graphics_context.f_ix=t_ix;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<290>";
		bb_graphics.bb_graphics_context.f_iy=t_iy;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<291>";
		bb_graphics.bb_graphics_context.f_jx=t_jx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<292>";
		bb_graphics.bb_graphics_context.f_jy=t_jy;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<293>";
		bb_graphics.bb_graphics_context.f_tx=t_tx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<294>";
		bb_graphics.bb_graphics_context.f_ty=t_ty;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<295>";
		bb_graphics.bb_graphics_context.f_tformed=((t_ix!=1.0f || t_iy!=0.0f || t_jx!=0.0f || t_jy!=1.0f || t_tx!=0.0f || t_ty!=0.0f)?1:0);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<296>";
		bb_graphics.bb_graphics_context.f_matDirty=1;
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_SetMatrix2(float[] t_m){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<285>";
		bb_graphics.bb_graphics_SetMatrix(t_m[0],t_m[1],t_m[2],t_m[3],t_m[4],t_m[5]);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_SetColor(float t_r,float t_g,float t_b){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<244>";
		bb_graphics.bb_graphics_context.f_color_r=t_r;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<245>";
		bb_graphics.bb_graphics_context.f_color_g=t_g;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<246>";
		bb_graphics.bb_graphics_context.f_color_b=t_b;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<247>";
		bb_graphics.bb_graphics_renderDevice.SetColor(t_r,t_g,t_b);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_SetAlpha(float t_alpha){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<255>";
		bb_graphics.bb_graphics_context.f_alpha=t_alpha;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<256>";
		bb_graphics.bb_graphics_renderDevice.SetAlpha(t_alpha);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_SetBlend(int t_blend){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<264>";
		bb_graphics.bb_graphics_context.f_blend=t_blend;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<265>";
		bb_graphics.bb_graphics_renderDevice.SetBlend(t_blend);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DeviceWidth(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<221>";
		int t_=bb_graphics.bb_graphics_device.Width();
		bb_std_lang.popErr();
		return t_;
	}
	public static int bb_graphics_DeviceHeight(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<225>";
		int t_=bb_graphics.bb_graphics_device.Height();
		bb_std_lang.popErr();
		return t_;
	}
	public static int bb_graphics_SetScissor(float t_x,float t_y,float t_width,float t_height){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<273>";
		bb_graphics.bb_graphics_context.f_scissor_x=t_x;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<274>";
		bb_graphics.bb_graphics_context.f_scissor_y=t_y;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<275>";
		bb_graphics.bb_graphics_context.f_scissor_width=t_width;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<276>";
		bb_graphics.bb_graphics_context.f_scissor_height=t_height;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<277>";
		bb_graphics.bb_graphics_renderDevice.SetScissor((int)(t_x),(int)(t_y),(int)(t_width),(int)(t_height));
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_BeginRender(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<206>";
		if(!((bb_graphics.bb_graphics_device.Mode())!=0)){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<206>";
			bb_std_lang.popErr();
			return 0;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<207>";
		bb_graphics.bb_graphics_renderDevice=bb_graphics.bb_graphics_device;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<208>";
		bb_graphics.bb_graphics_context.f_matrixSp=0;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<209>";
		bb_graphics.bb_graphics_SetMatrix(1.0f,0.0f,0.0f,1.0f,0.0f,0.0f);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<210>";
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<211>";
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<212>";
		bb_graphics.bb_graphics_SetBlend(0);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<213>";
		bb_graphics.bb_graphics_SetScissor(0.0f,0.0f,(float)(bb_graphics.bb_graphics_DeviceWidth()),(float)(bb_graphics.bb_graphics_DeviceHeight()));
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_EndRender(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<217>";
		bb_graphics.bb_graphics_renderDevice=null;
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DebugRenderDevice(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<48>";
		if(!((bb_graphics.bb_graphics_renderDevice)!=null)){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<48>";
			bb_std_lang.Error("Rendering operations can only be performed inside OnRender");
		}
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_Cls(float t_r,float t_g,float t_b){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<354>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<356>";
		bb_graphics.bb_graphics_renderDevice.Cls(t_r,t_g,t_b);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_Transform(float t_ix,float t_iy,float t_jx,float t_jy,float t_tx,float t_ty){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<331>";
		float t_ix2=t_ix*bb_graphics.bb_graphics_context.f_ix+t_iy*bb_graphics.bb_graphics_context.f_jx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<332>";
		float t_iy2=t_ix*bb_graphics.bb_graphics_context.f_iy+t_iy*bb_graphics.bb_graphics_context.f_jy;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<333>";
		float t_jx2=t_jx*bb_graphics.bb_graphics_context.f_ix+t_jy*bb_graphics.bb_graphics_context.f_jx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<334>";
		float t_jy2=t_jx*bb_graphics.bb_graphics_context.f_iy+t_jy*bb_graphics.bb_graphics_context.f_jy;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<335>";
		float t_tx2=t_tx*bb_graphics.bb_graphics_context.f_ix+t_ty*bb_graphics.bb_graphics_context.f_jx+bb_graphics.bb_graphics_context.f_tx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<336>";
		float t_ty2=t_tx*bb_graphics.bb_graphics_context.f_iy+t_ty*bb_graphics.bb_graphics_context.f_jy+bb_graphics.bb_graphics_context.f_ty;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<337>";
		bb_graphics.bb_graphics_SetMatrix(t_ix2,t_iy2,t_jx2,t_jy2,t_tx2,t_ty2);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_Transform2(float[] t_m){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<327>";
		bb_graphics.bb_graphics_Transform(t_m[0],t_m[1],t_m[2],t_m[3],t_m[4],t_m[5]);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_Scale(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<345>";
		bb_graphics.bb_graphics_Transform(t_x,0.0f,0.0f,t_y,0.0f,0.0f);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_Translate(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<341>";
		bb_graphics.bb_graphics_Transform(1.0f,0.0f,0.0f,1.0f,t_x,t_y);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DrawCircle(float t_x,float t_y,float t_r){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<393>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<395>";
		bb_graphics.bb_graphics_context.m_Validate();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<396>";
		bb_graphics.bb_graphics_renderDevice.DrawOval(t_x-t_r,t_y-t_r,t_r*2.0f,t_r*2.0f);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DrawRect(float t_x,float t_y,float t_w,float t_h){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<369>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<371>";
		bb_graphics.bb_graphics_context.m_Validate();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<372>";
		bb_graphics.bb_graphics_renderDevice.DrawRect(t_x,t_y,t_w,t_h);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DrawLine(float t_x1,float t_y1,float t_x2,float t_y2){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<377>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<379>";
		bb_graphics.bb_graphics_context.m_Validate();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<380>";
		bb_graphics.bb_graphics_renderDevice.DrawLine(t_x1,t_y1,t_x2,t_y2);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_PushMatrix(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<310>";
		int t_sp=bb_graphics.bb_graphics_context.f_matrixSp;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<311>";
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+0]=bb_graphics.bb_graphics_context.f_ix;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<312>";
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+1]=bb_graphics.bb_graphics_context.f_iy;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<313>";
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+2]=bb_graphics.bb_graphics_context.f_jx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<314>";
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+3]=bb_graphics.bb_graphics_context.f_jy;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<315>";
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+4]=bb_graphics.bb_graphics_context.f_tx;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<316>";
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+5]=bb_graphics.bb_graphics_context.f_ty;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<317>";
		bb_graphics.bb_graphics_context.f_matrixSp=t_sp+6;
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_PopMatrix(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<321>";
		int t_sp=bb_graphics.bb_graphics_context.f_matrixSp-6;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<322>";
		bb_graphics.bb_graphics_SetMatrix(bb_graphics.bb_graphics_context.f_matrixStack[t_sp+0],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+1],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+2],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+3],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+4],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+5]);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<323>";
		bb_graphics.bb_graphics_context.f_matrixSp=t_sp;
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DrawImage(bb_graphics_Image t_image,float t_x,float t_y,int t_frame){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<417>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<419>";
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<421>";
		if((bb_graphics.bb_graphics_context.f_tformed)!=0){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<422>";
			bb_graphics.bb_graphics_PushMatrix();
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<424>";
			bb_graphics.bb_graphics_Translate(t_x-t_image.f_tx,t_y-t_image.f_ty);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<426>";
			bb_graphics.bb_graphics_context.m_Validate();
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<428>";
			if((t_image.f_flags&65536)!=0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<429>";
				bb_graphics.bb_graphics_renderDevice.DrawSurface(t_image.f_surface,0.0f,0.0f);
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<431>";
				bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
			}
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<434>";
			bb_graphics.bb_graphics_PopMatrix();
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<436>";
			bb_graphics.bb_graphics_context.m_Validate();
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<438>";
			if((t_image.f_flags&65536)!=0){
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<439>";
				bb_graphics.bb_graphics_renderDevice.DrawSurface(t_image.f_surface,t_x-t_image.f_tx,t_y-t_image.f_ty);
			}else{
				bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<441>";
				bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,t_x-t_image.f_tx,t_y-t_image.f_ty,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
			}
		}
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_Rotate(float t_angle){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<349>";
		bb_graphics.bb_graphics_Transform((float)Math.Cos((t_angle)*bb_std_lang.D2R),-(float)Math.Sin((t_angle)*bb_std_lang.D2R),(float)Math.Sin((t_angle)*bb_std_lang.D2R),(float)Math.Cos((t_angle)*bb_std_lang.D2R),0.0f,0.0f);
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DrawImage2(bb_graphics_Image t_image,float t_x,float t_y,float t_rotation,float t_scaleX,float t_scaleY,int t_frame){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<448>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<450>";
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<452>";
		bb_graphics.bb_graphics_PushMatrix();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<454>";
		bb_graphics.bb_graphics_Translate(t_x,t_y);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<455>";
		bb_graphics.bb_graphics_Rotate(t_rotation);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<456>";
		bb_graphics.bb_graphics_Scale(t_scaleX,t_scaleY);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<458>";
		bb_graphics.bb_graphics_Translate(-t_image.f_tx,-t_image.f_ty);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<460>";
		bb_graphics.bb_graphics_context.m_Validate();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<462>";
		if((t_image.f_flags&65536)!=0){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<463>";
			bb_graphics.bb_graphics_renderDevice.DrawSurface(t_image.f_surface,0.0f,0.0f);
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<465>";
			bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<468>";
		bb_graphics.bb_graphics_PopMatrix();
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DrawImageRect(bb_graphics_Image t_image,float t_x,float t_y,int t_srcX,int t_srcY,int t_srcWidth,int t_srcHeight,int t_frame){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<473>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<475>";
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<477>";
		if((bb_graphics.bb_graphics_context.f_tformed)!=0){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<478>";
			bb_graphics.bb_graphics_PushMatrix();
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<481>";
			bb_graphics.bb_graphics_Translate(-t_image.f_tx+t_x,-t_image.f_ty+t_y);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<483>";
			bb_graphics.bb_graphics_context.m_Validate();
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<485>";
			bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<487>";
			bb_graphics.bb_graphics_PopMatrix();
		}else{
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<489>";
			bb_graphics.bb_graphics_context.m_Validate();
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<491>";
			bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,-t_image.f_tx+t_x,-t_image.f_ty+t_y,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
		}
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_graphics_DrawImageRect2(bb_graphics_Image t_image,float t_x,float t_y,int t_srcX,int t_srcY,int t_srcWidth,int t_srcHeight,float t_rotation,float t_scaleX,float t_scaleY,int t_frame){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<497>";
		bb_graphics.bb_graphics_DebugRenderDevice();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<499>";
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<501>";
		bb_graphics.bb_graphics_PushMatrix();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<503>";
		bb_graphics.bb_graphics_Translate(t_x,t_y);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<504>";
		bb_graphics.bb_graphics_Rotate(t_rotation);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<505>";
		bb_graphics.bb_graphics_Scale(t_scaleX,t_scaleY);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<506>";
		bb_graphics.bb_graphics_Translate(-t_image.f_tx,-t_image.f_ty);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<508>";
		bb_graphics.bb_graphics_context.m_Validate();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<510>";
		bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/graphics.monkey<512>";
		bb_graphics.bb_graphics_PopMatrix();
		bb_std_lang.popErr();
		return 0;
	}
}
class bb_graphicsdevice{
}
class bb_input{
	public static gxtkInput bb_input_device;
	public static int bb_input_SetInputDevice(gxtkInput t_dev){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<16>";
		bb_input.bb_input_device=t_dev;
		bb_std_lang.popErr();
		return 0;
	}
	public static int bb_input_KeyDown(int t_key){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<49>";
		int t_=bb_input.bb_input_device.KeyDown(t_key);
		bb_std_lang.popErr();
		return t_;
	}
	public static float bb_input_JoyX(int t_index,int t_unit){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<116>";
		float t_=bb_input.bb_input_device.JoyX(t_index|t_unit<<4);
		bb_std_lang.popErr();
		return t_;
	}
	public static float bb_input_JoyY(int t_index,int t_unit){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<120>";
		float t_=bb_input.bb_input_device.JoyY(t_index|t_unit<<4);
		bb_std_lang.popErr();
		return t_;
	}
	public static int bb_input_JoyDown(int t_button,int t_unit){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<128>";
		int t_=bb_input.bb_input_device.KeyDown(t_button|t_unit<<4|256);
		bb_std_lang.popErr();
		return t_;
	}
	public static int bb_input_MouseHit(int t_button){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<80>";
		int t_=bb_input.bb_input_device.KeyHit(1+t_button);
		bb_std_lang.popErr();
		return t_;
	}
	public static float bb_input_MouseX(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<68>";
		float t_=bb_input.bb_input_device.MouseX();
		bb_std_lang.popErr();
		return t_;
	}
	public static float bb_input_MouseY(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<72>";
		float t_=bb_input.bb_input_device.MouseY();
		bb_std_lang.popErr();
		return t_;
	}
	public static int bb_input_MouseDown(int t_button){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/mojo/input.monkey<76>";
		int t_=bb_input.bb_input_device.KeyDown(1+t_button);
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_inputdevice{
}
class bb_mojo{
}
class bb_boxes{
}
class bb_lang{
}
class bb_list{
}
class bb_map{
}
class bb_math{
	public static int bb_math_Clamp(int t_n,int t_min,int t_max){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<61>";
		if(t_n<t_min){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<61>";
			bb_std_lang.popErr();
			return t_min;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<62>";
		if(t_n>t_max){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<62>";
			bb_std_lang.popErr();
			return t_max;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<63>";
		bb_std_lang.popErr();
		return t_n;
	}
	public static float bb_math_Clamp2(float t_n,float t_min,float t_max){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<88>";
		if(t_n<t_min){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<88>";
			bb_std_lang.popErr();
			return t_min;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<89>";
		if(t_n>t_max){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<89>";
			bb_std_lang.popErr();
			return t_max;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<90>";
		bb_std_lang.popErr();
		return t_n;
	}
	public static int bb_math_Max(int t_x,int t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<56>";
		if(t_x>t_y){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<56>";
			bb_std_lang.popErr();
			return t_x;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<57>";
		bb_std_lang.popErr();
		return t_y;
	}
	public static float bb_math_Max2(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<83>";
		if(t_x>t_y){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<83>";
			bb_std_lang.popErr();
			return t_x;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<84>";
		bb_std_lang.popErr();
		return t_y;
	}
	public static int bb_math_Min(int t_x,int t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<51>";
		if(t_x<t_y){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<51>";
			bb_std_lang.popErr();
			return t_x;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<52>";
		bb_std_lang.popErr();
		return t_y;
	}
	public static float bb_math_Min2(float t_x,float t_y){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<78>";
		if(t_x<t_y){
			bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<78>";
			bb_std_lang.popErr();
			return t_x;
		}
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/math.monkey<79>";
		bb_std_lang.popErr();
		return t_y;
	}
}
class bb_monkey{
}
class bb_random{
	public static int bb_random_Seed;
	public static float bb_random_Rnd(){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/random.monkey<21>";
		bb_random.bb_random_Seed=bb_random.bb_random_Seed*1664525+1013904223|0;
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/random.monkey<22>";
		float t_=(float)(bb_random.bb_random_Seed>>8&16777215)/16777216.0f;
		bb_std_lang.popErr();
		return t_;
	}
	public static float bb_random_Rnd2(float t_low,float t_high){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/random.monkey<30>";
		float t_=bb_random.bb_random_Rnd3(t_high-t_low)+t_low;
		bb_std_lang.popErr();
		return t_;
	}
	public static float bb_random_Rnd3(float t_range){
		bb_std_lang.pushErr();
		bb_std_lang.errInfo="C:/apps/MonkeyPro66/modules/monkey/random.monkey<26>";
		float t_=bb_random.bb_random_Rnd()*t_range;
		bb_std_lang.popErr();
		return t_;
	}
}
class bb_set{
}
class bb_stack{
}
//${TRANSCODE_END}
