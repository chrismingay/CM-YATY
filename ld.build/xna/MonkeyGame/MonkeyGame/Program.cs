
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
public const String CONFIG="release";
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
		bb_app.bb_app_device=(new bb_app_AppDevice()).g_AppDevice_new(this);
		return this;
	}
	public virtual int m_OnCreate(){
		return 0;
	}
	public virtual int m_OnUpdate(){
		return 0;
	}
	public virtual int m_OnSuspend(){
		return 0;
	}
	public virtual int m_OnResume(){
		return 0;
	}
	public virtual int m_OnRender(){
		return 0;
	}
	public virtual int m_OnLoading(){
		return 0;
	}
}
class bb__LDApp : bb_app_App{
	public virtual bb__LDApp g_LDApp_new(){
		base.g_App_new();
		return this;
	}
	public static int g_ScreenHeight;
	public static int g_ScreenWidth;
	public static int g_RefreshRate;
	public static float g_Delta;
	public override int m_OnCreate(){
		bb_gfx_GFX.g_Init();
		bb_screenmanager_ScreenManager.g_Init();
		bb_sfx_SFX.g_Init();
		bb_controls_Controls.g_Init();
		bb_raztext_RazText.g_SetTextSheet(bb_graphics.bb_graphics_LoadImage("gfx/fonts.png",1,bb_graphics_Image.g_DefaultFlags));
		bb_screenmanager_ScreenManager.g_AddScreen("game",((new bb_gamescreen_GameScreen()).g_GameScreen_new()));
		bb_screenmanager_ScreenManager.g_AddScreen("title",((new bb_titlescreen_TitleScreen()).g_TitleScreen_new()));
		bb_sfx_SFX.g_AddMusic("ambient","ambient.mp3");
		bb_sfx_SFX.g_AddMusic("chase","chase.mp3");
		bb_screenmanager_ScreenManager.g_SetFadeColour(0.0f,0.0f,0.0f);
		bb_screenmanager_ScreenManager.g_SetFadeRate(0.1f);
		bb_screenmanager_ScreenManager.g_SetScreen("title");
		if(bb_controls_Controls.g_ControlMethod==2){
			g_RefreshRate=30;
		}else{
			g_RefreshRate=60;
		}
		bb_app.bb_app_SetUpdateRate(g_RefreshRate);
		g_Delta=(float)(g_RefreshRate)/60.0f;
		bb_autofit.bb_autofit_SetVirtualDisplay(g_ScreenWidth,g_ScreenHeight,1.0f);
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
		bb_controls_Controls.g_Update();
		g_ActualScreenX=(int)((float)(g_ActualScreenX)+(g_TargetScreenX-(float)(g_ActualScreenX))*g_ScreenMoveRate);
		g_ActualScreenY=(int)((float)(g_ActualScreenY)+(g_TargetScreenY-(float)(g_ActualScreenY))*g_ScreenMoveRate);
		g_ScreenX=g_ActualScreenX;
		g_ScreenY=g_ActualScreenY;
		bb_screenmanager_ScreenManager.g_Update();
		return 0;
	}
	public override int m_OnRender(){
		bb_autofit.bb_autofit_UpdateVirtualDisplay(true,true);
		bb_graphics.bb_graphics_Cls(0.0f,0.0f,0.0f);
		bb_screenmanager_ScreenManager.g_Render();
		return 0;
	}
	public static bb_level_Level g_level;
	public static void g_SetScreenTarget(float t_tX,float t_tY){
		g_TargetScreenX=t_tX-(float)(g_ScreenWidth)*0.5f;
		g_TargetScreenY=t_tY-(float)(g_ScreenHeight)*0.5f;
	}
	public static void g_SetScreenPosition(float t_tX,float t_tY){
		g_SetScreenTarget(t_tX,t_tY);
		g_ActualScreenX=(int)(g_TargetScreenX);
		g_ActualScreenY=(int)(g_TargetScreenY);
	}
}
class bb_app_AppDevice : gxtkApp{
	public bb_app_App f_app=null;
	public virtual bb_app_AppDevice g_AppDevice_new(bb_app_App t_app){
		this.f_app=t_app;
		bb_graphics.bb_graphics_SetGraphicsDevice(GraphicsDevice());
		bb_input.bb_input_SetInputDevice(InputDevice());
		bb_audio.bb_audio_SetAudioDevice(AudioDevice());
		return this;
	}
	public virtual bb_app_AppDevice g_AppDevice_new2(){
		return this;
	}
	public override int OnCreate(){
		bb_graphics.bb_graphics_SetFont(null,32);
		return f_app.m_OnCreate();
	}
	public override int OnUpdate(){
		return f_app.m_OnUpdate();
	}
	public override int OnSuspend(){
		return f_app.m_OnSuspend();
	}
	public override int OnResume(){
		return f_app.m_OnResume();
	}
	public override int OnRender(){
		bb_graphics.bb_graphics_BeginRender();
		int t_r=f_app.m_OnRender();
		bb_graphics.bb_graphics_EndRender();
		return t_r;
	}
	public override int OnLoading(){
		bb_graphics.bb_graphics_BeginRender();
		int t_r=f_app.m_OnLoading();
		bb_graphics.bb_graphics_EndRender();
		return t_r;
	}
	public int f_updateRate=0;
	public override int SetUpdateRate(int t_hertz){
		base.SetUpdateRate(t_hertz);
		f_updateRate=t_hertz;
		return 0;
	}
}
class bb_graphics_Image : Object{
	public static int g_DefaultFlags;
	public virtual bb_graphics_Image g_Image_new(){
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
		this.f_tx=t_tx;
		this.f_ty=t_ty;
		this.f_flags=this.f_flags&-2;
		return 0;
	}
	public virtual int m_ApplyFlags(int t_iflags){
		f_flags=t_iflags;
		if((f_flags&2)!=0){
			bb_graphics_Frame[] t_=f_frames;
			int t_2=0;
			while(t_2<bb_std_lang.length(t_)){
				bb_graphics_Frame t_f=t_[t_2];
				t_2=t_2+1;
				t_f.f_x+=1;
			}
			f_width-=2;
		}
		if((f_flags&4)!=0){
			bb_graphics_Frame[] t_3=f_frames;
			int t_4=0;
			while(t_4<bb_std_lang.length(t_3)){
				bb_graphics_Frame t_f2=t_3[t_4];
				t_4=t_4+1;
				t_f2.f_y+=1;
			}
			f_height-=2;
		}
		if((f_flags&1)!=0){
			m_SetHandle((float)(f_width)/2.0f,(float)(f_height)/2.0f);
		}
		if(bb_std_lang.length(f_frames)==1 && f_frames[0].f_x==0 && f_frames[0].f_y==0 && f_width==f_surface.Width() && f_height==f_surface.Height()){
			f_flags|=65536;
		}
		return 0;
	}
	public virtual bb_graphics_Image m_Init(gxtkSurface t_surf,int t_nframes,int t_iflags){
		f_surface=t_surf;
		f_width=f_surface.Width()/t_nframes;
		f_height=f_surface.Height();
		f_frames=new bb_graphics_Frame[t_nframes];
		for(int t_i=0;t_i<t_nframes;t_i=t_i+1){
			f_frames[t_i]=(new bb_graphics_Frame()).g_Frame_new(t_i*f_width,0);
		}
		m_ApplyFlags(t_iflags);
		return this;
	}
	public bb_graphics_Image f_source=null;
	public virtual bb_graphics_Image m_Grab(int t_x,int t_y,int t_iwidth,int t_iheight,int t_nframes,int t_iflags,bb_graphics_Image t_source){
		this.f_source=t_source;
		f_surface=t_source.f_surface;
		f_width=t_iwidth;
		f_height=t_iheight;
		f_frames=new bb_graphics_Frame[t_nframes];
		int t_ix=t_x;
		int t_iy=t_y;
		for(int t_i=0;t_i<t_nframes;t_i=t_i+1){
			if(t_ix+f_width>t_source.f_width){
				t_ix=0;
				t_iy+=f_height;
			}
			if(t_ix+f_width>t_source.f_width || t_iy+f_height>t_source.f_height){
				bb_std_lang.Error("Image frame outside surface");
			}
			f_frames[t_i]=(new bb_graphics_Frame()).g_Frame_new(t_ix+t_source.f_frames[0].f_x,t_iy+t_source.f_frames[0].f_y);
			t_ix+=f_width;
		}
		m_ApplyFlags(t_iflags);
		return this;
	}
	public virtual bb_graphics_Image m_GrabImage(int t_x,int t_y,int t_width,int t_height,int t_frames,int t_flags){
		if(bb_std_lang.length(this.f_frames)!=1){
			return null;
		}
		return ((new bb_graphics_Image()).g_Image_new()).m_Grab(t_x,t_y,t_width,t_height,t_frames,t_flags,this);
	}
	public virtual int m_Width(){
		return f_width;
	}
	public virtual int m_Height(){
		return f_height;
	}
	public virtual int m_Frames(){
		return bb_std_lang.length(f_frames);
	}
}
class bb_graphics_GraphicsContext : Object{
	public virtual bb_graphics_GraphicsContext g_GraphicsContext_new(){
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
		if((f_matDirty)!=0){
			bb_graphics.bb_graphics_renderDevice.SetMatrix(bb_graphics.bb_graphics_context.f_ix,bb_graphics.bb_graphics_context.f_iy,bb_graphics.bb_graphics_context.f_jx,bb_graphics.bb_graphics_context.f_jy,bb_graphics.bb_graphics_context.f_tx,bb_graphics.bb_graphics_context.f_ty);
			f_matDirty=0;
		}
		return 0;
	}
	public float[] f_matrixStack=new float[192];
}
class bb_graphics_Frame : Object{
	public int f_x=0;
	public int f_y=0;
	public virtual bb_graphics_Frame g_Frame_new(int t_x,int t_y){
		this.f_x=t_x;
		this.f_y=t_y;
		return this;
	}
	public virtual bb_graphics_Frame g_Frame_new2(){
		return this;
	}
}
class bb_gfx_GFX : Object{
	public static bb_graphics_Image g_Tileset;
	public static bb_graphics_Image g_Overlay;
	public static bb_graphics_Image g_Title;
	public static void g_Init(){
		g_Tileset=bb_graphics.bb_graphics_LoadImage("gfx/sheet.png",1,bb_graphics_Image.g_DefaultFlags);
		g_Overlay=bb_graphics.bb_graphics_LoadImage("gfx/overlay.png",1,bb_graphics_Image.g_DefaultFlags);
		g_Title=g_Tileset.m_GrabImage(345,448,167,64,1,1);
	}
	public static void g_Draw(bb_graphics_Image t_tImage,float t_tX,float t_tY,int t_tF,bool t_Follow){
		if(t_Follow==true){
			bb_graphics.bb_graphics_DrawImage(t_tImage,t_tX-(float)(bb__LDApp.g_ScreenX),t_tY-(float)(bb__LDApp.g_ScreenY),t_tF);
		}else{
			bb_graphics.bb_graphics_DrawImage(t_tImage,t_tX,t_tY,t_tF);
		}
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
		g_Screens=(new bb_map_StringMap()).g_StringMap_new();
		g_FadeAlpha=0.0f;
		g_FadeRate=0.01f;
		g_FadeRed=0.0f;
		g_FadeGreen=0.0f;
		g_FadeBlue=0.0f;
		g_FadeMode=0;
	}
	public static bb_gamescreen_GameScreen g_gameScreen;
	public static void g_AddScreen(String t_tName,bb_screen_Screen t_tScreen){
		g_Screens.m_Set(t_tName,t_tScreen);
		if(t_tName.CompareTo("game")==0){
			bb_screen_Screen t_=t_tScreen;
			g_gameScreen=(t_ is bb_gamescreen_GameScreen ? (bb_gamescreen_GameScreen)t_ : null);
		}
	}
	public static void g_SetFadeColour(float t_tR,float t_tG,float t_tB){
		g_FadeRed=bb_math.bb_math_Clamp2(t_tR,0.0f,255.0f);
		g_FadeGreen=bb_math.bb_math_Clamp2(t_tG,0.0f,255.0f);
		g_FadeBlue=bb_math.bb_math_Clamp2(t_tB,0.0f,255.0f);
	}
	public static void g_SetFadeRate(float t_tRate){
		g_FadeRate=bb_math.bb_math_Clamp2(t_tRate,0.001f,1.0f);
	}
	public static bb_screen_Screen g_ActiveScreen;
	public static String g_ActiveScreenName;
	public static void g_SetScreen(String t_tName){
		if(g_ActiveScreen!=null){
			g_ActiveScreen.m_OnScreenEnd();
		}
		g_ActiveScreen=g_Screens.m_Get(t_tName);
		g_ActiveScreen.m_OnScreenStart();
		g_FadeMode=1;
		g_ActiveScreenName=t_tName;
	}
	public static String g_NextScreenName;
	public static void g_Update(){
		if(g_ActiveScreen!=null){
			g_ActiveScreen.m_Update();
		}
		int t_=g_FadeMode;
		if(t_==0){
			g_FadeAlpha-=g_FadeRate;
			if(g_FadeAlpha<=0.0f){
				g_FadeAlpha=0.0f;
				g_FadeMode=1;
			}
		}else{
			if(t_==1){
			}else{
				if(t_==2){
					g_FadeAlpha+=g_FadeRate;
					if(g_FadeAlpha>=1.0f){
						g_FadeAlpha=1.0f;
						g_FadeMode=0;
						if(g_ActiveScreen!=null){
							g_ActiveScreen.m_OnScreenEnd();
						}
						g_ActiveScreenName=g_NextScreenName;
						g_ActiveScreen=g_Screens.m_Get(g_ActiveScreenName);
						g_ActiveScreen.m_OnScreenStart();
					}
				}
			}
		}
	}
	public static void g_Render(){
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		if(g_ActiveScreen!=null){
			g_ActiveScreen.m_Render();
		}
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		if(bb_controls_Controls.g_ControlMethod==2){
			bb_controls_Controls.g_TCMove.m_DoRenderRing();
			bb_controls_Controls.g_TCMove.m_DoRenderStick();
			bb_controls_Controls.g_TCAction1.m_Render();
			bb_controls_Controls.g_TCAction2.m_Render();
			bb_controls_Controls.g_TCEscapeKey.m_Render();
		}
		if(g_FadeMode!=1){
			bb_graphics.bb_graphics_SetColor(g_FadeRed,g_FadeGreen,g_FadeBlue);
			bb_graphics.bb_graphics_SetAlpha(g_FadeAlpha);
			bb_graphics.bb_graphics_DrawRect(0.0f,0.0f,(float)(bb_graphics.bb_graphics_DeviceWidth()),(float)(bb_graphics.bb_graphics_DeviceHeight()));
		}
	}
	public static void g_ChangeScreen(String t_tName){
		if(t_tName.CompareTo(g_ActiveScreenName)!=0){
			if(g_Screens.m_Contains(t_tName)){
				g_NextScreenName=t_tName;
				g_FadeMode=2;
			}
		}
	}
}
class bb_screen_Screen : Object{
	public virtual bb_screen_Screen g_Screen_new(){
		return this;
	}
	public virtual void m_OnScreenEnd(){
	}
	public virtual void m_OnScreenStart(){
	}
	public virtual void m_Update(){
	}
	public virtual void m_Render(){
	}
}
abstract class bb_map_Map : Object{
	public virtual bb_map_Map g_Map_new(){
		return this;
	}
	public bb_map_Node f_root=null;
	public abstract int m_Compare(String t_lhs,String t_rhs);
	public virtual int m_RotateLeft(bb_map_Node t_node){
		bb_map_Node t_child=t_node.f_right;
		t_node.f_right=t_child.f_left;
		if((t_child.f_left)!=null){
			t_child.f_left.f_parent=t_node;
		}
		t_child.f_parent=t_node.f_parent;
		if((t_node.f_parent)!=null){
			if(t_node==t_node.f_parent.f_left){
				t_node.f_parent.f_left=t_child;
			}else{
				t_node.f_parent.f_right=t_child;
			}
		}else{
			f_root=t_child;
		}
		t_child.f_left=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public virtual int m_RotateRight(bb_map_Node t_node){
		bb_map_Node t_child=t_node.f_left;
		t_node.f_left=t_child.f_right;
		if((t_child.f_right)!=null){
			t_child.f_right.f_parent=t_node;
		}
		t_child.f_parent=t_node.f_parent;
		if((t_node.f_parent)!=null){
			if(t_node==t_node.f_parent.f_right){
				t_node.f_parent.f_right=t_child;
			}else{
				t_node.f_parent.f_left=t_child;
			}
		}else{
			f_root=t_child;
		}
		t_child.f_right=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public virtual int m_InsertFixup(bb_map_Node t_node){
		while(((t_node.f_parent)!=null) && t_node.f_parent.f_color==-1 && ((t_node.f_parent.f_parent)!=null)){
			if(t_node.f_parent==t_node.f_parent.f_parent.f_left){
				bb_map_Node t_uncle=t_node.f_parent.f_parent.f_right;
				if(((t_uncle)!=null) && t_uncle.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle.f_color=1;
					t_uncle.f_parent.f_color=-1;
					t_node=t_uncle.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_right){
						t_node=t_node.f_parent;
						m_RotateLeft(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					m_RotateRight(t_node.f_parent.f_parent);
				}
			}else{
				bb_map_Node t_uncle2=t_node.f_parent.f_parent.f_left;
				if(((t_uncle2)!=null) && t_uncle2.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle2.f_color=1;
					t_uncle2.f_parent.f_color=-1;
					t_node=t_uncle2.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_left){
						t_node=t_node.f_parent;
						m_RotateRight(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					m_RotateLeft(t_node.f_parent.f_parent);
				}
			}
		}
		f_root.f_color=1;
		return 0;
	}
	public virtual bool m_Set(String t_key,bb_screen_Screen t_value){
		bb_map_Node t_node=f_root;
		bb_map_Node t_parent=null;
		int t_cmp=0;
		while((t_node)!=null){
			t_parent=t_node;
			t_cmp=m_Compare(t_key,t_node.f_key);
			if(t_cmp>0){
				t_node=t_node.f_right;
			}else{
				if(t_cmp<0){
					t_node=t_node.f_left;
				}else{
					t_node.f_value=t_value;
					return false;
				}
			}
		}
		t_node=(new bb_map_Node()).g_Node_new(t_key,t_value,-1,t_parent);
		if((t_parent)!=null){
			if(t_cmp>0){
				t_parent.f_right=t_node;
			}else{
				t_parent.f_left=t_node;
			}
			m_InsertFixup(t_node);
		}else{
			f_root=t_node;
		}
		return true;
	}
	public virtual bb_map_Node m_FindNode(String t_key){
		bb_map_Node t_node=f_root;
		while((t_node)!=null){
			int t_cmp=m_Compare(t_key,t_node.f_key);
			if(t_cmp>0){
				t_node=t_node.f_right;
			}else{
				if(t_cmp<0){
					t_node=t_node.f_left;
				}else{
					return t_node;
				}
			}
		}
		return t_node;
	}
	public virtual bb_screen_Screen m_Get(String t_key){
		bb_map_Node t_node=m_FindNode(t_key);
		if((t_node)!=null){
			return t_node.f_value;
		}
		return null;
	}
	public virtual bool m_Contains(String t_key){
		return m_FindNode(t_key)!=null;
	}
}
class bb_map_StringMap : bb_map_Map{
	public virtual bb_map_StringMap g_StringMap_new(){
		base.g_Map_new();
		return this;
	}
	public override int m_Compare(String t_lhs,String t_rhs){
		return t_lhs.CompareTo(t_rhs);
	}
}
class bb_sfx_SFX : Object{
	public static int g_ActiveChannel;
	public static bb_map_StringMap2 g_Sounds;
	public static bb_map_StringMap3 g_Musics;
	public static void g_Init(){
		g_ActiveChannel=0;
		g_Sounds=(new bb_map_StringMap2()).g_StringMap_new();
		g_Musics=(new bb_map_StringMap3()).g_StringMap_new();
	}
	public static void g_AddMusic(String t_tName,String t_tFile){
		g_Musics.m_Set2(t_tName,t_tFile);
	}
	public static bool g_MusicActive;
	public static String g_CurrentMusic;
	public static void g_Music(String t_tMus,int t_tLoop){
		if(g_MusicActive==false){
			return;
		}
		if(!g_Musics.m_Contains(t_tMus)){
			bb_std_lang.Error("Music "+t_tMus+" does not appear to exist");
		}
		if((t_tMus.CompareTo(g_CurrentMusic)!=0) || (bb_audio.bb_audio_MusicState()==-1 || bb_audio.bb_audio_MusicState()==0)){
			bb_audio.bb_audio_PlayMusic("mus/"+g_Musics.m_Get(t_tMus),t_tLoop);
			g_CurrentMusic=t_tMus;
		}
	}
}
class bb_audio_Sound : Object{
}
abstract class bb_map_Map2 : Object{
	public virtual bb_map_Map2 g_Map_new(){
		return this;
	}
}
class bb_map_StringMap2 : bb_map_Map2{
	public virtual bb_map_StringMap2 g_StringMap_new(){
		base.g_Map_new();
		return this;
	}
}
abstract class bb_map_Map3 : Object{
	public virtual bb_map_Map3 g_Map_new(){
		return this;
	}
	public bb_map_Node2 f_root=null;
	public abstract int m_Compare(String t_lhs,String t_rhs);
	public virtual int m_RotateLeft2(bb_map_Node2 t_node){
		bb_map_Node2 t_child=t_node.f_right;
		t_node.f_right=t_child.f_left;
		if((t_child.f_left)!=null){
			t_child.f_left.f_parent=t_node;
		}
		t_child.f_parent=t_node.f_parent;
		if((t_node.f_parent)!=null){
			if(t_node==t_node.f_parent.f_left){
				t_node.f_parent.f_left=t_child;
			}else{
				t_node.f_parent.f_right=t_child;
			}
		}else{
			f_root=t_child;
		}
		t_child.f_left=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public virtual int m_RotateRight2(bb_map_Node2 t_node){
		bb_map_Node2 t_child=t_node.f_left;
		t_node.f_left=t_child.f_right;
		if((t_child.f_right)!=null){
			t_child.f_right.f_parent=t_node;
		}
		t_child.f_parent=t_node.f_parent;
		if((t_node.f_parent)!=null){
			if(t_node==t_node.f_parent.f_right){
				t_node.f_parent.f_right=t_child;
			}else{
				t_node.f_parent.f_left=t_child;
			}
		}else{
			f_root=t_child;
		}
		t_child.f_right=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public virtual int m_InsertFixup2(bb_map_Node2 t_node){
		while(((t_node.f_parent)!=null) && t_node.f_parent.f_color==-1 && ((t_node.f_parent.f_parent)!=null)){
			if(t_node.f_parent==t_node.f_parent.f_parent.f_left){
				bb_map_Node2 t_uncle=t_node.f_parent.f_parent.f_right;
				if(((t_uncle)!=null) && t_uncle.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle.f_color=1;
					t_uncle.f_parent.f_color=-1;
					t_node=t_uncle.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_right){
						t_node=t_node.f_parent;
						m_RotateLeft2(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					m_RotateRight2(t_node.f_parent.f_parent);
				}
			}else{
				bb_map_Node2 t_uncle2=t_node.f_parent.f_parent.f_left;
				if(((t_uncle2)!=null) && t_uncle2.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle2.f_color=1;
					t_uncle2.f_parent.f_color=-1;
					t_node=t_uncle2.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_left){
						t_node=t_node.f_parent;
						m_RotateRight2(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					m_RotateLeft2(t_node.f_parent.f_parent);
				}
			}
		}
		f_root.f_color=1;
		return 0;
	}
	public virtual bool m_Set2(String t_key,String t_value){
		bb_map_Node2 t_node=f_root;
		bb_map_Node2 t_parent=null;
		int t_cmp=0;
		while((t_node)!=null){
			t_parent=t_node;
			t_cmp=m_Compare(t_key,t_node.f_key);
			if(t_cmp>0){
				t_node=t_node.f_right;
			}else{
				if(t_cmp<0){
					t_node=t_node.f_left;
				}else{
					t_node.f_value=t_value;
					return false;
				}
			}
		}
		t_node=(new bb_map_Node2()).g_Node_new(t_key,t_value,-1,t_parent);
		if((t_parent)!=null){
			if(t_cmp>0){
				t_parent.f_right=t_node;
			}else{
				t_parent.f_left=t_node;
			}
			m_InsertFixup2(t_node);
		}else{
			f_root=t_node;
		}
		return true;
	}
	public virtual bb_map_Node2 m_FindNode(String t_key){
		bb_map_Node2 t_node=f_root;
		while((t_node)!=null){
			int t_cmp=m_Compare(t_key,t_node.f_key);
			if(t_cmp>0){
				t_node=t_node.f_right;
			}else{
				if(t_cmp<0){
					t_node=t_node.f_left;
				}else{
					return t_node;
				}
			}
		}
		return t_node;
	}
	public virtual bool m_Contains(String t_key){
		return m_FindNode(t_key)!=null;
	}
	public virtual String m_Get(String t_key){
		bb_map_Node2 t_node=m_FindNode(t_key);
		if((t_node)!=null){
			return t_node.f_value;
		}
		return "";
	}
}
class bb_map_StringMap3 : bb_map_Map3{
	public virtual bb_map_StringMap3 g_StringMap_new(){
		base.g_Map_new();
		return this;
	}
	public override int m_Compare(String t_lhs,String t_rhs){
		return t_lhs.CompareTo(t_rhs);
	}
}
class bb_controls_Controls : Object{
	public static bb_virtualstick_MyStick g_TCMove;
	public static bb_touchbutton_TouchButton g_TCAction1;
	public static bb_touchbutton_TouchButton g_TCAction2;
	public static bb_touchbutton_TouchButton g_TCEscapeKey;
	public static bb_touchbutton_TouchButton[] g_TCButtons;
	public static void g_Init(){
		g_TCMove=(new bb_virtualstick_MyStick()).g_MyStick_new();
		g_TCMove.m_SetRing(50.0f,(float)(bb__LDApp.g_ScreenHeight-50),40.0f);
		g_TCMove.m_SetStick(0.0f,0.0f,15.0f);
		g_TCMove.m_SetDeadZone(0.2f);
		g_TCMove.m_SetTriggerDistance(5.0f);
		g_TCAction1=(new bb_touchbutton_TouchButton()).g_TouchButton_new(bb__LDApp.g_ScreenWidth-60,bb__LDApp.g_ScreenHeight-40,20,20);
		g_TCAction2=(new bb_touchbutton_TouchButton()).g_TouchButton_new(bb__LDApp.g_ScreenWidth-30,bb__LDApp.g_ScreenHeight-40,20,20);
		g_TCEscapeKey=(new bb_touchbutton_TouchButton()).g_TouchButton_new(bb__LDApp.g_ScreenWidth-20,0,20,20);
		g_TCButtons=new bb_touchbutton_TouchButton[3];
		g_TCButtons[0]=g_TCAction1;
		g_TCButtons[1]=g_TCAction2;
		g_TCButtons[2]=g_TCEscapeKey;
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
		if((bb_input.bb_input_KeyDown(g_LeftKey))!=0){
			if(g_LeftDown==false){
				g_LeftHit=true;
			}
			g_LeftDown=true;
		}else{
			g_LeftDown=false;
		}
		if((bb_input.bb_input_KeyDown(g_RightKey))!=0){
			if(g_RightDown==false){
				g_RightHit=true;
			}
			g_RightDown=true;
		}else{
			g_RightDown=false;
		}
		if((bb_input.bb_input_KeyDown(g_UpKey))!=0){
			if(g_UpDown==false){
				g_UpHit=true;
			}
			g_UpDown=true;
		}else{
			g_UpDown=false;
		}
		if((bb_input.bb_input_KeyDown(g_DownKey))!=0){
			if(g_DownDown==false){
				g_DownHit=true;
			}
			g_DownDown=true;
		}else{
			g_DownDown=false;
		}
		if((bb_input.bb_input_KeyDown(g_ActionKey))!=0){
			if(g_ActionDown==false){
				g_ActionHit=true;
			}
			g_ActionDown=true;
		}else{
			g_ActionDown=false;
		}
		if((bb_input.bb_input_KeyDown(g_Action2Key))!=0){
			if(g_Action2Down==false){
				g_Action2Hit=true;
			}
			g_Action2Down=true;
		}else{
			g_Action2Down=false;
		}
		if((bb_input.bb_input_KeyDown(g_EscapeKey))!=0){
			if(g_EscapeDown==false){
				g_EscapeHit=true;
			}
			g_EscapeDown=true;
		}else{
			g_EscapeDown=false;
		}
	}
	public static void g_UpdateJoypad(){
		if(bb_input.bb_input_JoyX(0,0)<-0.1f){
			if(g_LeftDown==false){
				g_LeftHit=true;
			}
			g_LeftDown=true;
		}else{
			g_LeftDown=false;
		}
		if(bb_input.bb_input_JoyX(0,0)>0.1f){
			if(g_RightDown==false){
				g_RightHit=true;
			}
			g_RightDown=true;
		}else{
			g_RightDown=false;
		}
		if(bb_input.bb_input_JoyY(0,0)>0.1f){
			if(g_UpDown==false){
				g_UpHit=true;
			}
			g_UpDown=true;
		}else{
			g_UpDown=false;
		}
		if(bb_input.bb_input_JoyY(0,0)<-0.1f){
			if(g_DownDown==false){
				g_DownHit=true;
			}
			g_DownDown=true;
		}else{
			g_DownDown=false;
		}
		if((bb_input.bb_input_JoyDown(0,0))!=0){
			if(g_ActionDown==false){
				g_ActionHit=true;
			}
			g_ActionDown=true;
		}else{
			g_ActionDown=false;
		}
		if((bb_input.bb_input_JoyDown(1,0))!=0){
			if(g_Action2Down==false){
				g_Action2Hit=true;
			}
			g_Action2Down=true;
		}else{
			g_Action2Down=false;
		}
		if((bb_input.bb_input_JoyDown(7,0))!=0){
			if(g_EscapeDown==false){
				g_EscapeHit=true;
			}
			g_EscapeDown=true;
		}else{
			g_EscapeDown=false;
		}
	}
	public static bool g_TouchPoint;
	public static void g_UpdateTouch(){
		g_TCAction1.f_Hit=false;
		g_TCAction2.f_Hit=false;
		g_TCEscapeKey.f_Hit=false;
		if((bb_input.bb_input_MouseHit(0))!=0){
			g_TouchPoint=true;
			g_TCMove.m_StartTouch(bb_autofit.bb_autofit_VMouseX(true),bb_autofit.bb_autofit_VMouseY(true),0);
			for(int t_i=0;t_i<=2;t_i=t_i+1){
				if(g_TCButtons[t_i].m_Check((int)(bb_autofit.bb_autofit_VMouseX(true)),(int)(bb_autofit.bb_autofit_VMouseY(true)))){
					g_TCButtons[t_i].f_Hit=true;
				}
			}
		}else{
			if((bb_input.bb_input_MouseDown(0))!=0){
				g_TCMove.m_UpdateTouch(bb_autofit.bb_autofit_VMouseX(true),bb_autofit.bb_autofit_VMouseY(true),0);
				for(int t_i2=0;t_i2<=2;t_i2=t_i2+1){
					if(g_TCButtons[t_i2].m_Check((int)(bb_autofit.bb_autofit_VMouseX(true)),(int)(bb_autofit.bb_autofit_VMouseY(true)))){
						g_TCButtons[t_i2].f_Down=true;
					}
				}
			}else{
				if(g_TouchPoint){
					g_TouchPoint=false;
					g_TCMove.m_StopTouch(0);
					for(int t_i3=0;t_i3<=2;t_i3=t_i3+1){
						g_TCButtons[t_i3].f_Down=false;
					}
				}
			}
		}
		if(g_TCMove.m_GetDX()<-0.1f){
			if(g_LeftDown==false){
				g_LeftHit=true;
			}
			g_LeftDown=true;
		}else{
			g_LeftDown=false;
		}
		if(g_TCMove.m_GetDX()>0.1f){
			if(g_RightDown==false){
				g_RightHit=true;
			}
			g_RightDown=true;
		}else{
			g_RightDown=false;
		}
		if(g_TCMove.m_GetDY()>0.1f){
			if(g_UpDown==false){
				g_UpHit=true;
			}
			g_UpDown=true;
		}else{
			g_UpDown=false;
		}
		if(g_TCMove.m_GetDY()<-0.1f){
			if(g_DownDown==false){
				g_DownHit=true;
			}
			g_DownDown=true;
		}else{
			g_DownDown=false;
		}
		if(g_TCAction1.f_Down){
			if(g_ActionDown==false){
				g_ActionHit=true;
			}
			g_ActionDown=true;
		}else{
			g_ActionDown=false;
		}
		if(g_TCAction2.f_Down){
			if(g_Action2Down==false){
				g_Action2Hit=true;
			}
			g_Action2Down=true;
		}else{
			g_Action2Down=false;
		}
		if(g_TCEscapeKey.f_Down){
			if(g_EscapeDown==false){
				g_EscapeHit=true;
			}
			g_EscapeDown=true;
		}else{
			g_EscapeDown=false;
		}
	}
	public static void g_Update(){
		g_LeftHit=false;
		g_RightHit=false;
		g_DownHit=false;
		g_UpHit=false;
		g_ActionHit=false;
		g_Action2Hit=false;
		g_EscapeHit=false;
		int t_=g_ControlMethod;
		if(t_==0){
			g_UpdateKeyboard();
		}else{
			if(t_==1){
				g_UpdateJoypad();
			}else{
				if(t_==2){
					g_UpdateTouch();
				}
			}
		}
	}
}
class bb_virtualstick_VirtualStick : Object{
	public virtual bb_virtualstick_VirtualStick g_VirtualStick_new(){
		return this;
	}
	public float f_ringX=.0f;
	public float f_ringY=.0f;
	public float f_ringRadius=.0f;
	public virtual void m_SetRing(float t_ringX,float t_ringY,float t_ringRadius){
		this.f_ringX=t_ringX;
		this.f_ringY=t_ringY;
		this.f_ringRadius=t_ringRadius;
	}
	public float f_stickX=0.0f;
	public float f_stickY=0.0f;
	public float f_stickRadius=.0f;
	public virtual void m_SetStick(float t_stickX,float t_stickY,float t_stickRadius){
		this.f_stickX=t_stickX;
		this.f_stickY=t_stickY;
		this.f_stickRadius=t_stickRadius;
	}
	public float f_deadZone=.0f;
	public virtual void m_SetDeadZone(float t_deadZone){
		this.f_deadZone=t_deadZone;
	}
	public float f_triggerDistance=-1.0f;
	public virtual void m_SetTriggerDistance(float t_triggerDistance){
		this.f_triggerDistance=t_triggerDistance;
	}
	public int f_touchNumber=-1;
	public float f_firstTouchX=.0f;
	public float f_firstTouchY=.0f;
	public bool f_triggered=false;
	public float f_stickPower=.0f;
	public float f_stickAngle=.0f;
	public virtual void m_UpdateStick(){
		if(f_touchNumber>=0){
			float t_length=(float)Math.Sqrt(f_stickX*f_stickX+f_stickY*f_stickY);
			f_stickPower=t_length/f_ringRadius;
			if(f_stickPower>1.0f){
				f_stickPower=1.0f;
			}
			if(f_stickPower<f_deadZone){
				f_stickPower=0.0f;
				f_stickAngle=0.0f;
				f_stickX=0.0f;
				f_stickY=0.0f;
			}else{
				if(f_stickX==0.0f && f_stickY==0.0f){
					f_stickAngle=0.0f;
					f_stickPower=0.0f;
				}else{
					if(f_stickX==0.0f && f_stickY>0.0f){
						f_stickAngle=90.0f;
					}else{
						if(f_stickX==0.0f && f_stickY<0.0f){
							f_stickAngle=270.0f;
						}else{
							if(f_stickY==0.0f && f_stickX>0.0f){
								f_stickAngle=0.0f;
							}else{
								if(f_stickY==0.0f && f_stickX<0.0f){
									f_stickAngle=180.0f;
								}else{
									if(f_stickX>0.0f && f_stickY>0.0f){
										f_stickAngle=(float)(Math.Atan(f_stickY/f_stickX)*bb_std_lang.R2D);
									}else{
										if(f_stickX<0.0f){
											f_stickAngle=180.0f+(float)(Math.Atan(f_stickY/f_stickX)*bb_std_lang.R2D);
										}else{
											f_stickAngle=360.0f+(float)(Math.Atan(f_stickY/f_stickX)*bb_std_lang.R2D);
										}
									}
								}
							}
						}
					}
				}
				if(t_length>f_ringRadius){
					f_stickPower=1.0f;
					f_stickX=(float)Math.Cos((f_stickAngle)*bb_std_lang.D2R)*f_ringRadius;
					f_stickY=(float)Math.Sin((f_stickAngle)*bb_std_lang.D2R)*f_ringRadius;
				}
			}
		}
	}
	public virtual void m_StartTouch(float t_x,float t_y,int t_touchnum){
		if(f_touchNumber<0){
			if((t_x-f_ringX)*(t_x-f_ringX)+(t_y-f_ringY)*(t_y-f_ringY)<=f_ringRadius*f_ringRadius){
				f_touchNumber=t_touchnum;
				f_firstTouchX=t_x;
				f_firstTouchY=t_y;
				f_triggered=false;
				if(f_triggerDistance<=0.0f){
					f_triggered=true;
					f_stickX=t_x-f_ringX;
					f_stickY=f_ringY-t_y;
				}
				m_UpdateStick();
			}
		}
	}
	public virtual void m_UpdateTouch(float t_x,float t_y,int t_touchnum){
		if(f_touchNumber>=0 && f_touchNumber==t_touchnum){
			if(!f_triggered){
				if((t_x-f_firstTouchX)*(t_x-f_firstTouchX)+(t_y-f_firstTouchY)*(t_y-f_firstTouchY)>f_triggerDistance*f_triggerDistance){
					f_triggered=true;
				}
			}
			if(f_triggered){
				f_stickX=t_x-f_ringX;
				f_stickY=f_ringY-t_y;
				m_UpdateStick();
			}
		}
	}
	public virtual void m_StopTouch(int t_touchnum){
		if(f_touchNumber>=0 && f_touchNumber==t_touchnum){
			f_touchNumber=-1;
			f_stickX=0.0f;
			f_stickY=0.0f;
			f_stickAngle=0.0f;
			f_stickPower=0.0f;
			f_triggered=false;
		}
	}
	public virtual float m_GetDX(){
		return (float)Math.Cos((f_stickAngle)*bb_std_lang.D2R)*f_stickPower;
	}
	public virtual float m_GetDY(){
		return (float)Math.Sin((f_stickAngle)*bb_std_lang.D2R)*f_stickPower;
	}
	public virtual void m_RenderRing(float t_x,float t_y){
		bb_graphics.bb_graphics_DrawCircle(t_x,t_y,f_ringRadius);
	}
	public virtual void m_DoRenderRing(){
		m_RenderRing(f_ringX,f_ringY);
	}
	public virtual void m_RenderStick(float t_x,float t_y){
		bb_graphics.bb_graphics_DrawCircle(t_x,t_y,f_stickRadius);
	}
	public virtual void m_DoRenderStick(){
		m_RenderStick(f_ringX+f_stickX,f_ringY-f_stickY);
	}
}
class bb_virtualstick_MyStick : bb_virtualstick_VirtualStick{
	public virtual bb_virtualstick_MyStick g_MyStick_new(){
		base.g_VirtualStick_new();
		return this;
	}
	public override void m_RenderRing(float t_x,float t_y){
		bb_graphics.bb_graphics_SetColor(0.0f,0.0f,0.0f);
		bb_graphics.bb_graphics_SetAlpha(0.1f);
		base.m_RenderRing(t_x,t_y);
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
	}
	public override void m_RenderStick(float t_x,float t_y){
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_graphics.bb_graphics_SetAlpha(0.5f);
		base.m_RenderStick(t_x,t_y);
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
	}
}
class bb_touchbutton_TouchButton : Object{
	public int f_X=0;
	public int f_Y=0;
	public int f_W=0;
	public int f_H=0;
	public virtual bb_touchbutton_TouchButton g_TouchButton_new(int t_tX,int t_tY,int t_tW,int t_tH){
		f_X=t_tX;
		f_Y=t_tY;
		f_W=t_tW;
		f_H=t_tH;
		return this;
	}
	public virtual bb_touchbutton_TouchButton g_TouchButton_new2(){
		return this;
	}
	public bool f_Hit=false;
	public virtual bool m_Check(int t_tX,int t_tY){
		return bb_functions.bb_functions_PointInRect((float)(t_tX),(float)(t_tY),(float)(f_X),(float)(f_Y),(float)(f_W),(float)(f_H));
	}
	public bool f_Down=false;
	public virtual void m_Render(){
		bb_graphics.bb_graphics_SetColor(0.0f,0.0f,0.0f);
		bb_graphics.bb_graphics_SetAlpha(0.25f);
		bb_graphics.bb_graphics_DrawRect((float)(f_X),(float)(f_Y),(float)(f_W),(float)(f_H));
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_graphics.bb_graphics_SetAlpha(0.5f);
		bb_gfx.bb_gfx_DrawHollowRect(f_X,f_Y,f_W,f_H);
	}
}
class bb_raztext_RazText : Object{
	public static bb_graphics_Image g_TextSheet;
	public static void g_SetTextSheet(bb_graphics_Image t_tImage){
		g_TextSheet=t_tImage;
	}
	public bb_list_List f_Lines=null;
	public virtual bb_raztext_RazText g_RazText_new(){
		f_Lines=(new bb_list_List()).g_List_new();
		return this;
	}
	public String f_OriginalString="";
	public virtual void m_AddLine(String t_tString){
		bb_raztext_RazChar[] t_tmp=new bb_raztext_RazChar[t_tString.Length];
		for(int t_i=0;t_i<=t_tString.Length-1;t_i=t_i+1){
			int t_let=(int)t_tString[t_i];
			String t_letstring=((String)bb_std_lang.slice(t_tString,t_i,t_i+1)).ToLower();
			int t_XOff=0;
			int t_YOff=0;
			String t_=t_letstring.ToLower();
			if(t_.CompareTo("a")==0){
				t_XOff=0;
				t_YOff=1;
			}else{
				if(t_.CompareTo("b")==0){
					t_XOff=1;
					t_YOff=1;
				}else{
					if(t_.CompareTo("c")==0){
						t_XOff=2;
						t_YOff=1;
					}else{
						if(t_.CompareTo("d")==0){
							t_XOff=3;
							t_YOff=1;
						}else{
							if(t_.CompareTo("e")==0){
								t_XOff=4;
								t_YOff=1;
							}else{
								if(t_.CompareTo("f")==0){
									t_XOff=5;
									t_YOff=1;
								}else{
									if(t_.CompareTo("g")==0){
										t_XOff=6;
										t_YOff=1;
									}else{
										if(t_.CompareTo("h")==0){
											t_XOff=7;
											t_YOff=1;
										}else{
											if(t_.CompareTo("i")==0){
												t_XOff=8;
												t_YOff=1;
											}else{
												if(t_.CompareTo("j")==0){
													t_XOff=9;
													t_YOff=1;
												}else{
													if(t_.CompareTo("k")==0){
														t_XOff=0;
														t_YOff=2;
													}else{
														if(t_.CompareTo("l")==0){
															t_XOff=1;
															t_YOff=2;
														}else{
															if(t_.CompareTo("m")==0){
																t_XOff=2;
																t_YOff=2;
															}else{
																if(t_.CompareTo("n")==0){
																	t_XOff=3;
																	t_YOff=2;
																}else{
																	if(t_.CompareTo("o")==0){
																		t_XOff=4;
																		t_YOff=2;
																	}else{
																		if(t_.CompareTo("p")==0){
																			t_XOff=5;
																			t_YOff=2;
																		}else{
																			if(t_.CompareTo("q")==0){
																				t_XOff=6;
																				t_YOff=2;
																			}else{
																				if(t_.CompareTo("r")==0){
																					t_XOff=7;
																					t_YOff=2;
																				}else{
																					if(t_.CompareTo("s")==0){
																						t_XOff=8;
																						t_YOff=2;
																					}else{
																						if(t_.CompareTo("t")==0){
																							t_XOff=9;
																							t_YOff=2;
																						}else{
																							if(t_.CompareTo("u")==0){
																								t_XOff=0;
																								t_YOff=3;
																							}else{
																								if(t_.CompareTo("v")==0){
																									t_XOff=1;
																									t_YOff=3;
																								}else{
																									if(t_.CompareTo("w")==0){
																										t_XOff=2;
																										t_YOff=3;
																									}else{
																										if(t_.CompareTo("x")==0){
																											t_XOff=3;
																											t_YOff=3;
																										}else{
																											if(t_.CompareTo("y")==0){
																												t_XOff=4;
																												t_YOff=3;
																											}else{
																												if(t_.CompareTo("z")==0){
																													t_XOff=5;
																													t_YOff=3;
																												}else{
																													if(t_.CompareTo("0")==0){
																														t_XOff=9;
																														t_YOff=0;
																													}else{
																														if(t_.CompareTo("1")==0){
																															t_XOff=0;
																															t_YOff=0;
																														}else{
																															if(t_.CompareTo("2")==0){
																																t_XOff=1;
																																t_YOff=0;
																															}else{
																																if(t_.CompareTo("3")==0){
																																	t_XOff=2;
																																	t_YOff=0;
																																}else{
																																	if(t_.CompareTo("4")==0){
																																		t_XOff=3;
																																		t_YOff=0;
																																	}else{
																																		if(t_.CompareTo("5")==0){
																																			t_XOff=4;
																																			t_YOff=0;
																																		}else{
																																			if(t_.CompareTo("6")==0){
																																				t_XOff=5;
																																				t_YOff=0;
																																			}else{
																																				if(t_.CompareTo("7")==0){
																																					t_XOff=6;
																																					t_YOff=0;
																																				}else{
																																					if(t_.CompareTo("8")==0){
																																						t_XOff=7;
																																						t_YOff=0;
																																					}else{
																																						if(t_.CompareTo("9")==0){
																																							t_XOff=8;
																																							t_YOff=0;
																																						}else{
																																							if(t_.CompareTo(",")==0){
																																								t_XOff=6;
																																								t_YOff=3;
																																							}else{
																																								if(t_.CompareTo(".")==0){
																																									t_XOff=7;
																																									t_YOff=3;
																																								}else{
																																									if(t_.CompareTo("!")==0){
																																										t_XOff=8;
																																										t_YOff=3;
																																									}else{
																																										if(t_.CompareTo("?")==0){
																																											t_XOff=9;
																																											t_YOff=3;
																																										}else{
																																											if(t_.CompareTo("@")==0){
																																												t_XOff=1;
																																												t_YOff=4;
																																											}else{
																																												if(t_.CompareTo(" ")==0){
																																													t_XOff=9;
																																													t_YOff=4;
																																												}else{
																																													if(t_.CompareTo("/")==0){
																																														t_XOff=0;
																																														t_YOff=4;
																																													}else{
																																														if(t_.CompareTo("-")==0){
																																															t_XOff=2;
																																															t_YOff=4;
																																														}else{
																																															if(t_.CompareTo(":")==0){
																																																t_XOff=3;
																																																t_YOff=4;
																																															}else{
																																																if(t_.CompareTo(";")==0){
																																																	t_XOff=4;
																																																	t_YOff=4;
																																																}else{
																																																	if(t_.CompareTo("_")==0){
																																																		t_XOff=5;
																																																		t_YOff=4;
																																																	}else{
																																																		if((t_.CompareTo("(")==0)){
																																																			t_XOff=6;
																																																			t_YOff=4;
																																																		}else{
																																																			if((t_.CompareTo(")")==0)){
																																																				t_XOff=7;
																																																				t_YOff=4;
																																																			}else{
																																																				if(t_.CompareTo("*")==0){
																																																					t_XOff=8;
																																																					t_YOff=4;
																																																				}else{
																																																					if(t_.CompareTo("+")==0){
																																																						t_XOff=9;
																																																						t_YOff=4;
																																																					}else{
																																																						t_XOff=9;
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
			t_tmp[t_i]=(new bb_raztext_RazChar()).g_RazChar_new();
			t_tmp[t_i].f_XOff=t_XOff;
			t_tmp[t_i].f_YOff=t_YOff;
			t_tmp[t_i].f_TextValue=t_letstring;
			if(t_XOff==9 && t_YOff==4){
				t_tmp[t_i].f_Visible=false;
			}
		}
		f_Lines.m_AddLast(t_tmp);
	}
	public virtual bb_raztext_RazText g_RazText_new2(String t_tString){
		f_Lines=(new bb_list_List()).g_List_new();
		f_OriginalString=t_tString;
		m_AddLine(t_tString);
		return this;
	}
	public virtual void m_AddMutliLines(String t_tString){
		String[] t_tmp=bb_std_lang.split(t_tString,"\r");
		for(int t_i=0;t_i<=bb_std_lang.length(t_tmp)-1;t_i=t_i+1){
			m_AddLine(t_tmp[t_i]);
		}
	}
	public int f_X=0;
	public int f_Y=0;
	public virtual void m_SetPos(int t_tX,int t_tY){
		f_X=t_tX;
		f_Y=t_tY;
	}
	public int f_VerticalSpacing=0;
	public int f_HorizontalSpacing=-2;
	public virtual void m_SetSpacing(int t_tX,int t_tY){
		f_VerticalSpacing=t_tY;
		f_HorizontalSpacing=t_tX;
	}
}
class bb_gamescreen_GameScreen : bb_screen_Screen{
	public virtual bb_gamescreen_GameScreen g_GameScreen_new(){
		base.g_Screen_new();
		return this;
	}
	public bb_level_Level f_level=null;
	public override void m_OnScreenStart(){
		bb__LDApp.g_level=bb_level.bb_level_GenerateLevel();
		f_level=bb__LDApp.g_level;
		f_level.m_Start();
	}
	public override void m_OnScreenEnd(){
	}
	public override void m_Update(){
		if(f_level!=null){
			f_level.m_Update();
		}
	}
	public override void m_Render(){
		bb_graphics.bb_graphics_Cls(255.0f,255.0f,255.0f);
		if(f_level!=null){
			f_level.m_Render();
		}
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
		this.f_key=t_key;
		this.f_value=t_value;
		this.f_color=t_color;
		this.f_parent=t_parent;
		return this;
	}
	public virtual bb_map_Node g_Node_new2(){
		return this;
	}
}
class bb_titlescreen_TitleScreen : bb_screen_Screen{
	public virtual bb_titlescreen_TitleScreen g_TitleScreen_new(){
		base.g_Screen_new();
		return this;
	}
	public override void m_OnScreenStart(){
	}
	public override void m_OnScreenEnd(){
	}
	public override void m_Update(){
		bb_random.bb_random_Rnd();
		if(bb_controls_Controls.g_ActionHit){
			bb_screenmanager_ScreenManager.g_ChangeScreen("game");
		}
	}
	public override void m_Render(){
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
		this.f_key=t_key;
		this.f_value=t_value;
		this.f_color=t_color;
		this.f_parent=t_parent;
		return this;
	}
	public virtual bb_map_Node2 g_Node_new2(){
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
		f_vwidth=(float)(t_width);
		f_vheight=(float)(t_height);
		f_vzoom=t_zoom;
		f_lastvzoom=f_vzoom+1.0f;
		f_vratio=f_vheight/f_vwidth;
		g_Display=this;
		return this;
	}
	public virtual bb_autofit_VirtualDisplay g_VirtualDisplay_new2(){
		return this;
	}
	public float f_multi=.0f;
	public virtual float m_VMouseX(bool t_limit){
		float t_mouseoffset=bb_input.bb_input_MouseX()-(float)(bb_graphics.bb_graphics_DeviceWidth())*0.5f;
		float t_x=t_mouseoffset/f_multi/f_vzoom+bb_autofit.bb_autofit_VDeviceWidth()*0.5f;
		if(t_limit){
			float t_widthlimit=f_vwidth-1.0f;
			if(t_x>0.0f){
				if(t_x<t_widthlimit){
					return t_x;
				}else{
					return t_widthlimit;
				}
			}else{
				return 0.0f;
			}
		}else{
			return t_x;
		}
	}
	public virtual float m_VMouseY(bool t_limit){
		float t_mouseoffset=bb_input.bb_input_MouseY()-(float)(bb_graphics.bb_graphics_DeviceHeight())*0.5f;
		float t_y=t_mouseoffset/f_multi/f_vzoom+bb_autofit.bb_autofit_VDeviceHeight()*0.5f;
		if(t_limit){
			float t_heightlimit=f_vheight-1.0f;
			if(t_y>0.0f){
				if(t_y<t_heightlimit){
					return t_y;
				}else{
					return t_heightlimit;
				}
			}else{
				return 0.0f;
			}
		}else{
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
		if(bb_graphics.bb_graphics_DeviceWidth()!=f_lastdevicewidth || bb_graphics.bb_graphics_DeviceHeight()!=f_lastdeviceheight){
			f_lastdevicewidth=bb_graphics.bb_graphics_DeviceWidth();
			f_lastdeviceheight=bb_graphics.bb_graphics_DeviceHeight();
			f_device_changed=1;
		}
		if((f_device_changed)!=0){
			f_fdw=(float)(bb_graphics.bb_graphics_DeviceWidth());
			f_fdh=(float)(bb_graphics.bb_graphics_DeviceHeight());
			f_dratio=f_fdh/f_fdw;
			if(f_dratio>f_vratio){
				f_multi=f_fdw/f_vwidth;
				f_heightborder=(f_fdh-f_vheight*f_multi)*0.5f;
				f_widthborder=0.0f;
			}else{
				f_multi=f_fdh/f_vheight;
				f_widthborder=(f_fdw-f_vwidth*f_multi)*0.5f;
				f_heightborder=0.0f;
			}
		}
		if(f_vzoom!=f_lastvzoom){
			f_lastvzoom=f_vzoom;
			f_zoom_changed=1;
		}
		if(((f_zoom_changed)!=0) || ((f_device_changed)!=0)){
			if(t_zoomborders){
				f_realx=f_vwidth*f_vzoom*f_multi;
				f_realy=f_vheight*f_vzoom*f_multi;
				f_offx=(f_fdw-f_realx)*0.5f;
				f_offy=(f_fdh-f_realy)*0.5f;
				if(t_keepborders){
					if(f_offx<f_widthborder){
						f_sx=f_widthborder;
						f_sw=f_fdw-f_widthborder*2.0f;
					}else{
						f_sx=f_offx;
						f_sw=f_fdw-f_offx*2.0f;
					}
					if(f_offy<f_heightborder){
						f_sy=f_heightborder;
						f_sh=f_fdh-f_heightborder*2.0f;
					}else{
						f_sy=f_offy;
						f_sh=f_fdh-f_offy*2.0f;
					}
				}else{
					f_sx=f_offx;
					f_sw=f_fdw-f_offx*2.0f;
					f_sy=f_offy;
					f_sh=f_fdh-f_offy*2.0f;
				}
				f_sx=bb_math.bb_math_Max2(0.0f,f_sx);
				f_sy=bb_math.bb_math_Max2(0.0f,f_sy);
				f_sw=bb_math.bb_math_Min2(f_sw,f_fdw);
				f_sh=bb_math.bb_math_Min2(f_sh,f_fdh);
			}else{
				f_sx=bb_math.bb_math_Max2(0.0f,f_widthborder);
				f_sy=bb_math.bb_math_Max2(0.0f,f_heightborder);
				f_sw=bb_math.bb_math_Min2(f_fdw-f_widthborder*2.0f,f_fdw);
				f_sh=bb_math.bb_math_Min2(f_fdh-f_heightborder*2.0f,f_fdh);
			}
			f_scaledw=f_vwidth*f_multi*f_vzoom;
			f_scaledh=f_vheight*f_multi*f_vzoom;
			f_vxoff=(f_fdw-f_scaledw)*0.5f;
			f_vyoff=(f_fdh-f_scaledh)*0.5f;
			f_vxoff=f_vxoff/f_multi/f_vzoom;
			f_vyoff=f_vyoff/f_multi/f_vzoom;
			f_device_changed=0;
			f_zoom_changed=0;
		}
		bb_graphics.bb_graphics_SetScissor(0.0f,0.0f,(float)(bb_graphics.bb_graphics_DeviceWidth()),(float)(bb_graphics.bb_graphics_DeviceHeight()));
		bb_graphics.bb_graphics_Cls(0.0f,0.0f,0.0f);
		bb_graphics.bb_graphics_SetScissor(f_sx,f_sy,f_sw,f_sh);
		bb_graphics.bb_graphics_Scale(f_multi*f_vzoom,f_multi*f_vzoom);
		if((f_vzoom)!=0.0f){
			bb_graphics.bb_graphics_Translate(f_vxoff,f_vyoff);
		}
		return 0;
	}
}
class bb_level_Level : Object{
	public int f_controlledYeti=0;
	public bb_raztext_RazText f_txtWait=null;
	public float f_titleFade=0.0f;
	public int f_titleFadeMode=0;
	public int f_titleFadeTimer=0;
	public virtual bb_level_Level g_Level_new(){
		bb_entity_Entity.g_Init();
		bb_dog_Dog.g_Init(bb__LDApp.g_level);
		bb_yeti_Yeti.g_Init(bb__LDApp.g_level);
		bb_skier_Skier.g_Init(bb__LDApp.g_level);
		bb_tree_Tree.g_Init(bb__LDApp.g_level);
		bb_rock_Rock.g_Init(bb__LDApp.g_level);
		bb_flag_Flag.g_Init(bb__LDApp.g_level);
		f_controlledYeti=bb_yeti_Yeti.g_Create(0.0f,0.0f);
		bb_yeti_Yeti.g_a[f_controlledYeti].m_StartWaiting();
		bb_dog_Dog.g_Create(50.0f,50.0f);
		int t_firstSkier=bb_skier_Skier.g_Create(50.0f,-200.0f);
		bb_skier_Skier.g_a[t_firstSkier].f_TargetYeti=0;
		bb_skier_Skier.g_a[t_firstSkier].m_StartPreTeasing();
		bb__LDApp.g_SetScreenPosition(bb_yeti_Yeti.g_a[f_controlledYeti].f_X,bb_yeti_Yeti.g_a[f_controlledYeti].f_Y);
		f_txtWait=(new bb_raztext_RazText()).g_RazText_new();
		f_txtWait.m_AddMutliLines(bb_app.bb_app_LoadString("txt/wait.txt").Replace("\n",""));
		f_txtWait.m_SetPos(96,320);
		f_txtWait.m_SetSpacing(-3,-1);
		f_titleFade=0.0f;
		f_titleFadeMode=0;
		f_titleFadeTimer=0;
		return this;
	}
	public static int g_Width;
	public static int g_Height;
	public virtual void m_Start(){
		bb_sfx_SFX.g_Music("ambient",1);
	}
	public virtual void m_UpdateTitleFade(){
		int t_=f_titleFadeMode;
		if(t_==0){
			f_titleFadeTimer+=1;
			if(f_titleFadeTimer>=120){
				f_titleFadeTimer=0;
				f_titleFadeMode=1;
			}
		}else{
			if(t_==1){
				f_titleFade+=0.02f;
				if(f_titleFade>=1.0f){
					f_titleFade=1.0f;
					f_titleFadeMode=2;
				}
			}else{
				if(t_==2){
					f_titleFadeTimer+=1;
					if(f_titleFadeTimer>=240){
						f_titleFadeTimer=0;
						f_titleFadeMode=3;
					}
				}else{
					if(t_==3){
						f_titleFade-=0.01f;
						if(f_titleFade<=0.0f){
							f_titleFade=0.0f;
							f_titleFadeMode=4;
						}
					}
				}
			}
		}
	}
	public virtual void m_Update(){
		bb__LDApp.g_SetScreenTarget(bb_yeti_Yeti.g_a[f_controlledYeti].f_X,bb_yeti_Yeti.g_a[f_controlledYeti].f_Y+(float)(bb__LDApp.g_ScreenHeight)*0.15f);
		bb_dog_Dog.g_UpdateAll();
		bb_yeti_Yeti.g_UpdateAll();
		bb_skier_Skier.g_UpdateAll();
		bb_tree_Tree.g_UpdateAll();
		bb_rock_Rock.g_UpdateAll();
		m_UpdateTitleFade();
	}
	public virtual void m_RenderGui(){
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0f,1.0f,504,0,8,360,0);
		bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,6.0f,1.0f+bb_yeti_Yeti.g_a[f_controlledYeti].f_Y/50.0f,464,0,10,10,0);
		for(int t_i=0;t_i<1;t_i=t_i+1){
			if(bb_skier_Skier.g_a[t_i].f_Active==true){
				bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,6.0f,1.0f+bb_skier_Skier.g_a[t_i].f_Y/50.0f,480,0,10,10,0);
			}
		}
	}
	public virtual void m_RenderTitleFade(){
		if(f_titleFadeMode>=1 && f_titleFadeMode<4){
			bb_graphics.bb_graphics_SetAlpha(f_titleFade);
			bb_graphics.bb_graphics_DrawImage(bb_gfx_GFX.g_Title,(float)(bb__LDApp.g_ScreenWidth)*0.5f,(float)(bb__LDApp.g_ScreenHeight)*0.5f,0);
			bb_graphics.bb_graphics_SetAlpha(1.0f);
		}
	}
	public virtual void m_Render(){
		bb_graphics.bb_graphics_SetColor(0.0f,0.0f,0.0f);
		bb_graphics.bb_graphics_SetAlpha(0.5f);
		bb_graphics.bb_graphics_DrawImage(bb_gfx_GFX.g_Overlay,0.0f,0.0f,0);
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		m_RenderGui();
		bb_dog_Dog.g_RenderAll();
		bb_yeti_Yeti.g_RenderAll();
		bb_skier_Skier.g_RenderAll();
		bb_tree_Tree.g_RenderAll();
		bb_rock_Rock.g_RenderAll();
		bb_flag_Flag.g_RenderAll();
		m_RenderTitleFade();
	}
}
class bb_entity_Entity : Object{
	public static bb_entity_Entity[][] g_a;
	public static void g_Init(){
		g_a=new bb_entity_Entity[9][];
		for(int t_i=0;t_i<9;t_i=t_i+1){
		}
	}
	public bb_level_Level f_level=null;
	public float f_W=.0f;
	public float f_H=.0f;
	public virtual bb_entity_Entity g_Entity_new(bb_level_Level t_tLev){
		f_level=t_tLev;
		f_W=16.0f;
		f_H=16.0f;
		return this;
	}
	public virtual bb_entity_Entity g_Entity_new2(){
		return this;
	}
	public int f_EnType=0;
	public bool f_Active=false;
	public virtual void m_Activate(){
		f_Active=true;
	}
	public float f_X=.0f;
	public float f_Y=.0f;
	public virtual void m_SetPosition(float t_tX,float t_tY){
		f_X=t_tX;
		f_Y=t_tY;
	}
	public float f_YS=.0f;
	public float f_Z=.0f;
	public bool f_onFloor=false;
	public virtual void m_Update(){
		if(f_Z>=0.0f){
			f_onFloor=true;
			f_Z=0.0f;
		}else{
			f_onFloor=false;
		}
	}
	public virtual bool m_IsOnScreen(int t_tAdditionalBuffer){
		return bb_functions.bb_functions_RectOverRect(f_X,f_Y,f_W,f_H,(float)(bb__LDApp.g_ScreenX-50-t_tAdditionalBuffer),(float)(bb__LDApp.g_ScreenY-50-t_tAdditionalBuffer),(float)(bb__LDApp.g_ScreenWidth+100+t_tAdditionalBuffer*2),(float)(bb__LDApp.g_ScreenHeight+100+t_tAdditionalBuffer*2));
	}
	public virtual void m_Deactivate(){
		f_Active=false;
	}
	public float f_XS=.0f;
	public float f_ZS=.0f;
	public virtual int m_CheckForCollision(){
		if(f_Z<-2.0f){
			return -1;
		}
		for(int t_Type=0;t_Type<9;t_Type=t_Type+1){
			if(f_EnType!=t_Type){
				int t_l=bb_std_lang.length(g_a[t_Type]);
				for(int t_i=0;t_i<t_l;t_i=t_i+1){
					if(g_a[t_Type][t_i].f_Active==true){
						if(bb_functions.bb_functions_RectOverRect(f_X,f_Y,f_W,f_H,g_a[t_Type][t_i].f_X,g_a[t_Type][t_i].f_Y,g_a[t_Type][t_i].f_W,g_a[t_Type][t_i].f_H)){
							return t_Type;
						}
					}
				}
			}
		}
		return -1;
	}
	public virtual void m_Render(){
	}
}
class bb_entity_EntityType : Object{
}
class bb_dog_Dog : bb_entity_Entity{
	public static bb_dog_Dog[] g_a;
	public virtual bb_dog_Dog g_Dog_new(bb_level_Level t_tLev){
		base.g_Entity_new2();
		f_level=t_tLev;
		f_EnType=1;
		f_W=8.0f;
		f_H=8.0f;
		return this;
	}
	public virtual bb_dog_Dog g_Dog_new2(){
		base.g_Entity_new2();
		return this;
	}
	public static bb_graphics_Image g_gfxStandFront;
	public static void g_Init(bb_level_Level t_tLev){
		g_a=new bb_dog_Dog[10];
		bb_entity_Entity.g_a[1]=new bb_entity_Entity[10];
		for(int t_i=0;t_i<10;t_i=t_i+1){
			g_a[t_i]=(new bb_dog_Dog()).g_Dog_new(t_tLev);
			bb_entity_Entity.g_a[1][t_i]=(g_a[t_i]);
		}
		g_gfxStandFront=bb_gfx_GFX.g_Tileset.m_GrabImage(0,80,16,16,1,1);
	}
	public static int g_NextDog;
	public override void m_Activate(){
		base.m_Activate();
	}
	public static int g_Create(float t_tX,float t_tY){
		int t_tDog=g_NextDog;
		g_a[g_NextDog].m_Activate();
		g_a[g_NextDog].m_SetPosition(t_tX,t_tY);
		g_NextDog+=1;
		if(g_NextDog==10){
			g_NextDog=0;
		}
		return t_tDog;
	}
	public override void m_Update(){
		if(!m_IsOnScreen(0)){
			m_Deactivate();
		}
	}
	public static void g_UpdateAll(){
		for(int t_i=0;t_i<10;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override void m_Render(){
		bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y,0,true);
	}
	public static void g_RenderAll(){
		for(int t_i=0;t_i<10;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_yeti_Yeti : bb_entity_Entity{
	public static bb_yeti_Yeti[] g_a;
	public virtual bb_yeti_Yeti g_Yeti_new(bb_level_Level t_tLev){
		base.g_Entity_new2();
		f_level=t_tLev;
		f_EnType=8;
		f_W=20.0f;
		f_H=30.0f;
		return this;
	}
	public virtual bb_yeti_Yeti g_Yeti_new2(){
		base.g_Entity_new2();
		return this;
	}
	public static bb_graphics_Image g_gfxStandFront;
	public static bb_graphics_Image g_gfxRunFront;
	public static bb_graphics_Image g_gfxRunLeft;
	public static bb_graphics_Image g_gfxRunRight;
	public static bb_graphics_Image g_gfxShadow;
	public static void g_Init(bb_level_Level t_tLev){
		g_a=new bb_yeti_Yeti[1];
		bb_entity_Entity.g_a[8]=new bb_entity_Entity[1];
		for(int t_i=0;t_i<1;t_i=t_i+1){
			g_a[t_i]=(new bb_yeti_Yeti()).g_Yeti_new(t_tLev);
			bb_entity_Entity.g_a[8][t_i]=(g_a[t_i]);
		}
		g_gfxStandFront=bb_gfx_GFX.g_Tileset.m_GrabImage(48,0,22,32,2,1);
		g_gfxRunFront=bb_gfx_GFX.g_Tileset.m_GrabImage(48,32,22,32,2,1);
		g_gfxRunLeft=bb_gfx_GFX.g_Tileset.m_GrabImage(48,64,22,32,2,1);
		g_gfxRunRight=bb_gfx_GFX.g_Tileset.m_GrabImage(48,96,22,32,2,1);
		g_gfxShadow=bb_gfx_GFX.g_Tileset.m_GrabImage(112,0,16,6,1,1);
	}
	public static int g_NextYeti;
	public int f_Status=0;
	public override void m_Activate(){
		base.m_Activate();
		f_Status=0;
	}
	public static int g_Create(float t_tX,float t_tY){
		int t_thisYeti=g_NextYeti;
		g_a[t_thisYeti].m_Activate();
		g_NextYeti+=1;
		if(g_NextYeti==1){
			g_NextYeti=0;
		}
		return t_thisYeti;
	}
	public virtual void m_StartWaiting(){
		f_Status=0;
	}
	public float f_aniRunFrameTimer=0.0f;
	public int f_aniRunFrame=0;
	public float f_aniWaitFrameTimer=0.0f;
	public int f_aniWaitFrame=0;
	public int f_D=0;
	public virtual void m_UpdateControlled(){
		if(bb_controls_Controls.g_LeftHit){
			if(f_D>0){
				f_D-=1;
			}else{
				if(f_onFloor){
					f_XS=-1.0f;
				}
			}
		}
		if(bb_controls_Controls.g_RightHit){
			if(f_D<6){
				f_D+=1;
			}else{
				if(f_onFloor){
					f_XS=1.0f;
				}
			}
		}
		if(bb_controls_Controls.g_DownHit){
			f_D=3;
		}
		if(bb_controls_Controls.g_UpHit){
			if(f_onFloor){
				f_ZS=-1.5f;
				f_XS*=1.5f;
				f_YS*=1.5f;
			}
		}
	}
	public float f_MaxYS=.0f;
	public float f_TargetXS=.0f;
	public virtual void m_UpdateChasing(){
		m_UpdateControlled();
		if(f_onFloor){
			int t_=f_D;
			if(t_==0){
				f_MaxYS=0.0f;
				f_TargetXS=0.0f;
			}else{
				if(t_==1){
					f_MaxYS=1.0f;
					f_TargetXS=-2.0f;
				}else{
					if(t_==2){
						f_MaxYS=2.0f;
						f_TargetXS=-1.0f;
					}else{
						if(t_==3){
							f_MaxYS=3.0f;
							f_TargetXS=0.0f;
						}else{
							if(t_==4){
								f_MaxYS=2.0f;
								f_TargetXS=1.0f;
							}else{
								if(t_==5){
									f_MaxYS=1.0f;
									f_TargetXS=2.0f;
								}else{
									if(t_==6){
										f_MaxYS=0.0f;
										f_TargetXS=0.0f;
									}
								}
							}
						}
					}
				}
			}
			if(f_YS>f_MaxYS-0.05f && f_YS<f_MaxYS+0.05f){
				f_YS=f_MaxYS;
			}else{
				if(f_YS>f_MaxYS){
					float t_tR=0.02f;
					if(f_D==0 || f_D==6){
						t_tR=0.04f;
					}
					f_YS*=1.0f-t_tR*bb__LDApp.g_Delta;
				}else{
					f_YS+=0.05f*bb__LDApp.g_Delta;
				}
			}
			if(f_XS>f_TargetXS-0.05f*bb__LDApp.g_Delta && f_XS<f_TargetXS+0.05f*bb__LDApp.g_Delta){
				f_XS=f_TargetXS;
			}else{
				if(f_XS<f_TargetXS){
					f_XS+=0.05f*bb__LDApp.g_Delta;
				}else{
					if(f_XS>f_TargetXS){
						f_XS-=0.05f*bb__LDApp.g_Delta;
					}
				}
			}
		}else{
			f_ZS+=0.05f*bb__LDApp.g_Delta;
		}
		f_X+=f_XS*bb__LDApp.g_Delta;
		f_Y+=f_YS*bb__LDApp.g_Delta;
		f_Z+=f_ZS*bb__LDApp.g_Delta;
	}
	public virtual void m_UpdateDazed(){
	}
	public virtual void m_UpdateEating(){
	}
	public virtual void m_UpdateWaitingHappy(){
	}
	public virtual void m_StartChasing(){
		f_D=3;
		f_XS=0.0f;
		f_YS=0.5f;
		f_ZS=-1.0f;
		f_Z=0.0f;
		f_Status=1;
	}
	public virtual void m_UpdateWaiting(){
		if(bb_controls_Controls.g_DownHit){
			m_StartChasing();
		}
	}
	public override void m_Update(){
		base.m_Update();
		f_aniRunFrameTimer+=1.0f*bb__LDApp.g_Delta;
		if(f_aniRunFrameTimer>=5.0f){
			f_aniRunFrameTimer=0.0f;
			f_aniRunFrame+=1;
			if(f_aniRunFrame>=2){
				f_aniRunFrame=0;
			}
		}
		f_aniWaitFrameTimer+=1.0f*bb__LDApp.g_Delta;
		if(f_aniWaitFrameTimer>=30.0f){
			f_aniWaitFrameTimer=0.0f;
			f_aniWaitFrame+=1;
			if(f_aniWaitFrame>=2){
				f_aniWaitFrame=0;
			}
		}
		int t_=f_Status;
		if(t_==1){
			m_UpdateChasing();
		}else{
			if(t_==3){
				m_UpdateDazed();
			}else{
				if(t_==2){
					m_UpdateEating();
				}else{
					if(t_==4){
						m_UpdateWaitingHappy();
					}else{
						if(t_==0){
							m_UpdateWaiting();
						}
					}
				}
			}
		}
		if(!m_IsOnScreen(0)){
			m_Deactivate();
		}
	}
	public static void g_UpdateAll(){
		for(int t_i=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public virtual void m_RenderChasing(){
		int t_=f_D;
		if(t_==0){
			bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y+f_Z,f_aniWaitFrame,true);
		}else{
			if(t_==1 || t_==2){
				bb_gfx_GFX.g_Draw(g_gfxRunLeft,f_X,f_Y+f_Z,f_aniRunFrame,true);
			}else{
				if(t_==3){
					bb_gfx_GFX.g_Draw(g_gfxRunFront,f_X,f_Y+f_Z,f_aniRunFrame,true);
				}else{
					if(t_==4 || t_==5){
						bb_gfx_GFX.g_Draw(g_gfxRunRight,f_X,f_Y+f_Z,f_aniRunFrame,true);
					}else{
						if(t_==6){
							bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y+f_Z,f_aniWaitFrame,true);
						}
					}
				}
			}
		}
	}
	public virtual void m_RenderDazed(){
	}
	public virtual void m_RenderEating(){
	}
	public virtual void m_RenderWaitingHappy(){
	}
	public virtual void m_RenderWaiting(){
		bb_gfx_GFX.g_Draw(g_gfxStandFront,f_X,f_Y,f_aniWaitFrame,true);
	}
	public override void m_Render(){
		bb_graphics.bb_graphics_SetAlpha(0.25f);
		bb_gfx_GFX.g_Draw(g_gfxShadow,f_X,f_Y+12.0f,0,true);
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		int t_=f_Status;
		if(t_==1){
			m_RenderChasing();
		}else{
			if(t_==3){
				m_RenderDazed();
			}else{
				if(t_==2){
					m_RenderEating();
				}else{
					if(t_==4){
						m_RenderWaitingHappy();
					}else{
						if(t_==0){
							m_RenderWaiting();
						}else{
							if(t_==5){
							}
						}
					}
				}
			}
		}
	}
	public static void g_RenderAll(){
		for(int t_i=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_skier_Skier : bb_entity_Entity{
	public static bb_skier_Skier[] g_a;
	public int f_TargetYeti=0;
	public virtual bb_skier_Skier g_Skier_new(bb_level_Level t_tLev){
		base.g_Entity_new2();
		f_level=t_tLev;
		f_EnType=5;
		f_W=12.0f;
		f_H=16.0f;
		f_TargetYeti=0;
		return this;
	}
	public virtual bb_skier_Skier g_Skier_new2(){
		base.g_Entity_new2();
		return this;
	}
	public static bb_graphics_Image g_gfxSki;
	public static bb_graphics_Image g_gfxTease;
	public static bb_graphics_Image g_gfxShadow;
	public static bb_graphics_Image g_gfxFall;
	public static void g_Init(bb_level_Level t_tLev){
		g_a=new bb_skier_Skier[1];
		bb_entity_Entity.g_a[5]=new bb_entity_Entity[1];
		for(int t_i=0;t_i<1;t_i=t_i+1){
			g_a[t_i]=(new bb_skier_Skier()).g_Skier_new(t_tLev);
			bb_entity_Entity.g_a[5][t_i]=(g_a[t_i]);
		}
		g_gfxSki=bb_gfx_GFX.g_Tileset.m_GrabImage(0,128,16,32,7,1);
		g_gfxTease=bb_gfx_GFX.g_Tileset.m_GrabImage(112,128,16,32,2,1);
		g_gfxShadow=bb_gfx_GFX.g_Tileset.m_GrabImage(0,160,16,4,1,1);
		g_gfxFall=bb_gfx_GFX.g_Tileset.m_GrabImage(144,128,16,32,4,1);
	}
	public static int g_NextSkier;
	public override void m_Activate(){
		base.m_Activate();
		f_TargetYeti=-1;
	}
	public static int g_Create(float t_tX,float t_tY){
		int t_tI=g_NextSkier;
		g_a[t_tI].m_Activate();
		g_a[t_tI].m_SetPosition(t_tX,t_tY);
		g_NextSkier+=1;
		if(g_NextSkier>=1){
			g_NextSkier=0;
		}
		return t_tI;
	}
	public int f_Status=0;
	public int f_D=0;
	public virtual void m_StartPreTeasing(){
		f_Status=5;
		f_D=3;
		f_YS=1.6f;
	}
	public virtual void m_UpdateDazed(){
	}
	public virtual void m_UpdateDead(){
	}
	public float f_fallTimer=.0f;
	public float f_fallFrameTimer=.0f;
	public int f_fallFrame=0;
	public virtual void m_ContinueSkiing(){
		f_Status=0;
		f_Z=0.0f;
		f_D=3;
		f_ZS=0.0f;
	}
	public virtual void m_UpdateFalling(){
		f_fallTimer+=1.0f*bb__LDApp.g_Delta;
		f_fallFrameTimer+=1.0f*bb__LDApp.g_Delta;
		if(f_fallFrameTimer>=7.0f){
			f_fallFrameTimer=0.0f;
			f_fallFrame+=1;
			if(f_fallFrame==4){
				f_fallFrame=0;
			}
		}
		if(f_onFloor==true){
			if(f_ZS>=4.0f){
				f_ZS=0.0f-f_ZS*0.5f;
				f_XS=f_XS*0.6f;
				f_YS=f_YS*0.6f;
			}else{
				if(f_fallTimer>=15.0f){
					m_ContinueSkiing();
				}else{
					bb_std_lang.Print((f_fallTimer).ToString(CultureInfo.InvariantCulture));
				}
			}
		}
		f_X+=f_XS*bb__LDApp.g_Delta;
		f_Y+=f_YS*bb__LDApp.g_Delta;
		f_Z+=f_ZS*bb__LDApp.g_Delta;
		f_ZS+=0.05f*bb__LDApp.g_Delta;
	}
	public float f_MaxYS=.0f;
	public float f_TargetXS=.0f;
	public virtual void m_StartFalling(){
		float t_tZ=(bb_math.bb_math_Abs2(f_XS)+bb_math.bb_math_Abs2(f_YS))*0.5f;
		f_ZS=0.0f-t_tZ;
		f_Status=1;
		f_fallTimer=0.0f;
	}
	public virtual void m_UpdateSkiing(){
		if(bb_random.bb_random_Rnd()<0.02f){
			if(bb_random.bb_random_Rnd()<0.5f){
				f_D-=1;
			}else{
				f_D+=1;
			}
		}
		f_D=bb_math.bb_math_Clamp(f_D,1,5);
		if(f_onFloor){
			int t_=f_D;
			if(t_==0){
				f_MaxYS=0.0f;
				f_TargetXS=0.0f;
			}else{
				if(t_==1){
					f_MaxYS=1.0f;
					f_TargetXS=-2.0f;
				}else{
					if(t_==2){
						f_MaxYS=2.0f;
						f_TargetXS=-1.0f;
					}else{
						if(t_==3){
							f_MaxYS=3.2f;
							f_TargetXS=0.0f;
						}else{
							if(t_==4){
								f_MaxYS=2.0f;
								f_TargetXS=1.0f;
							}else{
								if(t_==5){
									f_MaxYS=1.0f;
									f_TargetXS=2.0f;
								}else{
									if(t_==6){
										f_MaxYS=0.0f;
										f_TargetXS=0.0f;
									}
								}
							}
						}
					}
				}
			}
			if(f_YS>f_MaxYS-0.05f && f_YS<f_MaxYS+0.05f){
				f_YS=f_MaxYS;
			}else{
				if(f_YS>f_MaxYS){
					f_YS*=1.0f-0.02f*bb__LDApp.g_Delta;
				}else{
					if(f_YS<f_MaxYS){
						f_YS+=0.05f*bb__LDApp.g_Delta;
					}
				}
			}
			if(f_XS>f_TargetXS-0.05f*bb__LDApp.g_Delta && f_XS<f_TargetXS+0.05f*bb__LDApp.g_Delta){
				f_XS=f_TargetXS;
			}else{
				if(f_XS<f_TargetXS){
					f_XS+=0.05f*bb__LDApp.g_Delta;
				}else{
					if(f_XS>f_TargetXS){
						f_XS-=0.05f*bb__LDApp.g_Delta;
					}
				}
			}
		}else{
			f_ZS+=0.05f*bb__LDApp.g_Delta;
		}
		f_X+=f_XS*bb__LDApp.g_Delta;
		f_Y+=f_YS*bb__LDApp.g_Delta;
		f_Z+=f_ZS*bb__LDApp.g_Delta;
		int t_colStatus=m_CheckForCollision();
		int t_2=t_colStatus;
		if(t_2==0){
			f_ZS=-1.0f;
		}else{
			if(t_2==1){
				f_ZS=-1.0f;
				f_XS*=0.9f;
				f_YS*=0.9f;
			}else{
				if(t_2==2){
					f_XS*=0.9f;
					f_YS*=0.9f;
				}else{
					if(t_2==3){
						f_XS=f_XS*2.0f;
						f_YS=f_YS*2.0f;
						f_ZS=-3.0f;
					}else{
						if(t_2==4){
							m_StartFalling();
							f_XS*=0.5f;
							f_YS*=0.5f;
						}else{
							if(t_2==6){
							}else{
								if(t_2==7){
									m_StartFalling();
									f_XS*=0.7f;
									f_YS*=0.7f;
								}else{
									if(t_2==8){
									}
								}
							}
						}
					}
				}
			}
		}
	}
	public float f_teasingFrameTimer=.0f;
	public int f_teasingFrame=0;
	public virtual void m_StartSkiing(){
		f_Status=0;
		f_Z=0.0f;
		f_D=3;
		f_YS=4.0f;
		f_XS=0.0f;
		f_ZS=-2.0f;
		bb_sfx_SFX.g_Music("chase",1);
	}
	public virtual void m_UpdateTeasing(){
		if(f_YS==0.0f){
			if(f_teasingFrameTimer>0.0f){
				f_teasingFrameTimer-=1.0f*bb__LDApp.g_Delta;
				f_teasingFrame=1;
			}else{
				f_teasingFrameTimer=0.0f;
				f_teasingFrame=0;
			}
			if(bb_random.bb_random_Rnd()<0.04f){
				f_Y=f_Y+1.0f;
				f_teasingFrameTimer=5.0f;
			}
		}else{
			f_Y+=f_YS*bb__LDApp.g_Delta;
		}
		if(f_YS<=0.1f){
			f_YS=0.0f;
		}else{
			f_YS*=1.0f-0.05f*bb__LDApp.g_Delta;
		}
		if(bb_yeti_Yeti.g_a[f_TargetYeti].f_Y-f_Y<2.0f){
			m_StartSkiing();
		}else{
			if(bb_yeti_Yeti.g_a[f_TargetYeti].f_Y-f_Y<50.0f){
				if(bb_random.bb_random_Rnd()<0.02f){
					m_StartSkiing();
				}
			}
		}
	}
	public virtual int m_FindNearestYeti(){
		int t_nIndex=-1;
		float t_nDistance=99999999.0f;
		for(int t_i=0;t_i<1;t_i=t_i+1){
			if(bb_yeti_Yeti.g_a[t_i].f_Active==true){
				if(bb_yeti_Yeti.g_a[t_i].f_Y>f_Y){
					float t_tDist=bb_yeti_Yeti.g_a[t_i].f_Y-f_Y;
					if(t_tDist<200.0f){
						if(t_tDist<t_nDistance){
							t_nIndex=t_i;
							t_nDistance=t_tDist;
						}
					}
				}
			}
		}
		return t_nIndex;
	}
	public virtual void m_StartTeasing(){
		f_Status=4;
		f_D=3;
		f_TargetYeti=m_FindNearestYeti();
	}
	public virtual void m_UpdatePreTeasing(){
		f_Y+=f_YS*bb__LDApp.g_Delta;
		if(f_TargetYeti>=0){
			if(bb_yeti_Yeti.g_a[f_TargetYeti].f_Y-f_Y<130.0f){
				if(bb_random.bb_random_Rnd()<0.02f){
					f_D=0;
					m_StartTeasing();
				}
			}
		}
	}
	public override void m_Update(){
		base.m_Update();
		int t_=f_Status;
		if(t_==2){
			m_UpdateDazed();
		}else{
			if(t_==3){
				m_UpdateDead();
			}else{
				if(t_==1){
					m_UpdateFalling();
				}else{
					if(t_==0){
						m_UpdateSkiing();
					}else{
						if(t_==4){
							m_UpdateTeasing();
						}else{
							if(t_==5){
								m_UpdatePreTeasing();
							}
						}
					}
				}
			}
		}
	}
	public static void g_UpdateAll(){
		for(int t_i=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override void m_Render(){
		if(f_Z<0.0f){
			bb_graphics.bb_graphics_SetAlpha(0.25f);
			bb_gfx_GFX.g_Draw(g_gfxShadow,f_X-f_Z*1.0f,f_Y+7.0f-f_Z*2.0f,0,true);
		}
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		int t_=f_Status;
		if(t_==4){
			bb_gfx_GFX.g_Draw(g_gfxTease,f_X,f_Y+f_Z,f_teasingFrame,true);
		}else{
			if(t_==5){
				bb_gfx_GFX.g_Draw(g_gfxSki,f_X,f_Y+f_Z,f_D,true);
			}else{
				if(t_==1){
					bb_gfx_GFX.g_Draw(g_gfxFall,f_X,f_Y+f_Z,f_fallFrame,true);
				}else{
					bb_gfx_GFX.g_Draw(g_gfxSki,f_X,f_Y+f_Z,f_D,true);
				}
			}
		}
		bb_graphics.bb_graphics_DrawText((f_YS).ToString(CultureInfo.InvariantCulture),10.0f,10.0f,0.0f,0.0f);
		bb_graphics.bb_graphics_DrawText((f_TargetYeti).ToString(),10.0f,30.0f,0.0f,0.0f);
	}
	public virtual void m_RenderMarker(){
		int t_dX=(int)(f_X-(float)(bb__LDApp.g_ScreenX));
		int t_dY=(int)(f_Y-(float)(bb__LDApp.g_ScreenY));
		int t_hState=0;
		int t_vState=0;
		if(t_dX<0){
			t_hState=-1;
		}else{
			if(t_dX>bb__LDApp.g_ScreenWidth){
				t_hState=1;
			}
		}
		if(t_dY<0){
			t_vState=-1;
		}else{
			if(t_dY>bb__LDApp.g_ScreenHeight){
				t_vState=1;
			}
		}
		int t_=t_hState;
		if(t_==-1){
			int t_2=t_vState;
			if(t_2==-1){
				bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0f,1.0f,16,192,8,8,0);
			}else{
				if(t_2==0){
					bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0f,(float)(t_dY-4),0,176,8,9,0);
				}else{
					if(t_2==1){
						bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0f,(float)(bb__LDApp.g_ScreenHeight-9),0,192,8,8,0);
					}
				}
			}
		}else{
			if(t_==0){
				int t_3=t_vState;
				if(t_3==-1){
					bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(float)(t_dX-4),1.0f,32,176,9,8,0);
				}else{
					if(t_3==0){
					}else{
						if(t_3==1){
							bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(float)(t_dX-4),(float)(bb__LDApp.g_ScreenHeight-10),16,176,9,8,0);
						}
					}
				}
			}else{
				if(t_==1){
					int t_4=t_vState;
					if(t_4==-1){
						bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(float)(bb__LDApp.g_ScreenWidth-9),1.0f,24,192,8,8,0);
					}else{
						if(t_4==0){
							bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(float)(bb__LDApp.g_ScreenWidth-9),(float)(t_dY-4),8,176,8,9,0);
						}else{
							if(t_4==1){
								bb_graphics.bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(float)(bb__LDApp.g_ScreenWidth-9),(float)(bb__LDApp.g_ScreenHeight-9),8,192,8,8,0);
							}
						}
					}
				}
			}
		}
	}
	public static void g_RenderAll(){
		for(int t_i=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Render();
				g_a[t_i].m_RenderMarker();
			}
		}
	}
}
class bb_tree_Tree : bb_entity_Entity{
	public static bb_tree_Tree[] g_a;
	public virtual bb_tree_Tree g_Tree_new(bb_level_Level t_tLev){
		base.g_Entity_new2();
		f_level=t_tLev;
		f_EnType=7;
		f_W=10.0f;
		f_H=10.0f;
		return this;
	}
	public virtual bb_tree_Tree g_Tree_new2(){
		base.g_Entity_new2();
		return this;
	}
	public static bb_graphics_Image g_gfxSmallTree;
	public static bb_graphics_Image g_gfxBigTree;
	public static bb_graphics_Image g_gfxTreeStump;
	public static void g_Init(bb_level_Level t_tLev){
		g_a=new bb_tree_Tree[1000];
		bb_entity_Entity.g_a[7]=new bb_entity_Entity[1000];
		for(int t_i=0;t_i<1000;t_i=t_i+1){
			g_a[t_i]=(new bb_tree_Tree()).g_Tree_new(t_tLev);
			bb_entity_Entity.g_a[7][t_i]=(g_a[t_i]);
		}
		g_gfxSmallTree=bb_gfx_GFX.g_Tileset.m_GrabImage(32,256,16,16,1,1);
		g_gfxBigTree=bb_gfx_GFX.g_Tileset.m_GrabImage(0,240,16,32,1,1);
		g_gfxTreeStump=bb_gfx_GFX.g_Tileset.m_GrabImage(16,256,16,16,1,1);
	}
	public static int g_NextTree;
	public override void m_Activate(){
		base.m_Activate();
	}
	public int f_Type=0;
	public static int g_Create(float t_tX,float t_tY){
		int t_tT=g_NextTree;
		g_a[g_NextTree].m_Activate();
		g_a[g_NextTree].m_SetPosition(t_tX,t_tY);
		float t_chance=bb_random.bb_random_Rnd();
		int t_tType=0;
		if(t_chance<0.5f){
			t_tType=0;
		}else{
			if(t_chance<0.8f){
				t_tType=1;
			}else{
				t_tType=2;
			}
		}
		g_a[g_NextTree].f_Type=t_tType;
		g_NextTree+=1;
		if(g_NextTree==1000){
			g_NextTree=0;
		}
		return t_tT;
	}
	public override void m_Update(){
	}
	public static void g_UpdateAll(){
		for(int t_i=0;t_i<1000;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override void m_Render(){
		int t_=f_Type;
		if(t_==0){
			bb_gfx_GFX.g_Draw(g_gfxBigTree,f_X,f_Y,0,true);
		}else{
			if(t_==1){
				bb_gfx_GFX.g_Draw(g_gfxSmallTree,f_X,f_Y,0,true);
			}else{
				if(t_==2){
					bb_gfx_GFX.g_Draw(g_gfxTreeStump,f_X,f_Y,0,true);
				}
			}
		}
	}
	public static void g_RenderAll(){
		for(int t_i=0;t_i<1000;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_rock_Rock : bb_entity_Entity{
	public static bb_rock_Rock[] g_a;
	public virtual bb_rock_Rock g_Rock_new(bb_level_Level t_tLev){
		base.g_Entity_new2();
		f_level=t_tLev;
		f_EnType=4;
		f_W=10.0f;
		f_H=10.0f;
		return this;
	}
	public virtual bb_rock_Rock g_Rock_new2(){
		base.g_Entity_new2();
		return this;
	}
	public static bb_graphics_Image g_gfxRockBoulder;
	public static bb_graphics_Image g_gfxRockSpikey;
	public static bb_graphics_Image g_gfxRockFlat;
	public static void g_Init(bb_level_Level t_tLev){
		g_a=new bb_rock_Rock[300];
		bb_entity_Entity.g_a[4]=new bb_entity_Entity[300];
		for(int t_i=0;t_i<300;t_i=t_i+1){
			g_a[t_i]=(new bb_rock_Rock()).g_Rock_new(t_tLev);
			bb_entity_Entity.g_a[4][t_i]=(g_a[t_i]);
		}
		g_gfxRockBoulder=bb_gfx_GFX.g_Tileset.m_GrabImage(0,288,16,16,1,1);
		g_gfxRockSpikey=bb_gfx_GFX.g_Tileset.m_GrabImage(16,288,16,16,1,1);
		g_gfxRockFlat=bb_gfx_GFX.g_Tileset.m_GrabImage(32,288,16,16,1,1);
	}
	public static int g_NextRock;
	public int f_Type=0;
	public override void m_Activate(){
		base.m_Activate();
	}
	public static int g_Create(float t_tX,float t_tY){
		int t_tRock=g_NextRock;
		float t_chance=bb_random.bb_random_Rnd();
		int t_tType=0;
		if(t_chance<0.33f){
			t_tType=0;
		}else{
			if(t_chance<0.66f){
				t_tType=1;
			}else{
				t_tType=2;
			}
		}
		g_a[g_NextRock].f_Type=t_tType;
		g_a[g_NextRock].m_Activate();
		g_a[g_NextRock].m_SetPosition(t_tX,t_tY);
		g_NextRock+=1;
		if(g_NextRock==300){
			g_NextRock=0;
		}
		return t_tRock;
	}
	public override void m_Update(){
	}
	public static void g_UpdateAll(){
		for(int t_i=0;t_i<300;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override void m_Render(){
		int t_=f_Type;
		if(t_==0){
			bb_gfx_GFX.g_Draw(g_gfxRockBoulder,f_X,f_Y,0,true);
		}else{
			if(t_==2){
				bb_gfx_GFX.g_Draw(g_gfxRockFlat,f_X,f_Y,0,true);
			}else{
				if(t_==1){
					bb_gfx_GFX.g_Draw(g_gfxRockSpikey,f_X,f_Y,0,true);
				}
			}
		}
	}
	public static void g_RenderAll(){
		for(int t_i=0;t_i<300;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_flag_Flag : bb_entity_Entity{
	public static bb_flag_Flag[] g_a;
	public virtual bb_flag_Flag g_Flag_new(bb_level_Level t_tLev){
		base.g_Entity_new2();
		f_level=t_tLev;
		f_EnType=2;
		f_W=8.0f;
		f_H=16.0f;
		return this;
	}
	public virtual bb_flag_Flag g_Flag_new2(){
		base.g_Entity_new2();
		return this;
	}
	public static bb_graphics_Image g_gfxFlag;
	public static int g_NextFlag;
	public static void g_Init(bb_level_Level t_tLev){
		g_a=new bb_flag_Flag[150];
		bb_entity_Entity.g_a[2]=new bb_entity_Entity[150];
		for(int t_i=0;t_i<150;t_i=t_i+1){
			g_a[t_i]=(new bb_flag_Flag()).g_Flag_new(t_tLev);
			bb_entity_Entity.g_a[2][t_i]=(g_a[t_i]);
		}
		g_gfxFlag=bb_gfx_GFX.g_Tileset.m_GrabImage(0,272,16,16,2,1);
		g_NextFlag=0;
	}
	public override void m_Activate(){
		base.m_Activate();
	}
	public static int g_Create(float t_tX,float t_tY){
		int t_tFlag=g_NextFlag;
		g_a[g_NextFlag].m_Activate();
		g_a[g_NextFlag].m_SetPosition(t_tX,t_tY);
		g_NextFlag+=1;
		if(g_NextFlag==150){
			g_NextFlag=0;
		}
		return t_tFlag;
	}
	public int f_Type=0;
	public override void m_Render(){
		bb_gfx_GFX.g_Draw(g_gfxFlag,f_X,f_Y,f_Type,true);
	}
	public static void g_RenderAll(){
		for(int t_i=0;t_i<150;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
	public override void m_Update(){
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
		return this;
	}
	public int f_XOff=0;
	public int f_YOff=0;
	public String f_TextValue="";
	public bool f_Visible=true;
}
class bb_list_List : Object{
	public virtual bb_list_List g_List_new(){
		return this;
	}
	public bb_list_Node f__head=((new bb_list_HeadNode()).g_HeadNode_new());
	public virtual bb_list_Node m_AddLast(bb_raztext_RazChar[] t_data){
		return (new bb_list_Node()).g_Node_new(f__head,f__head.f__pred,t_data);
	}
	public virtual bb_list_List g_List_new2(bb_raztext_RazChar[][] t_data){
		bb_raztext_RazChar[][] t_=t_data;
		int t_2=0;
		while(t_2<bb_std_lang.length(t_)){
			bb_raztext_RazChar[] t_t=t_[t_2];
			t_2=t_2+1;
			m_AddLast(t_t);
		}
		return this;
	}
}
class bb_list_Node : Object{
	public bb_list_Node f__succ=null;
	public bb_list_Node f__pred=null;
	public bb_raztext_RazChar[] f__data=new bb_raztext_RazChar[0];
	public virtual bb_list_Node g_Node_new(bb_list_Node t_succ,bb_list_Node t_pred,bb_raztext_RazChar[] t_data){
		f__succ=t_succ;
		f__pred=t_pred;
		f__succ.f__pred=this;
		f__pred.f__succ=this;
		f__data=t_data;
		return this;
	}
	public virtual bb_list_Node g_Node_new2(){
		return this;
	}
}
class bb_list_HeadNode : bb_list_Node{
	public virtual bb_list_HeadNode g_HeadNode_new(){
		base.g_Node_new2();
		f__succ=(this);
		f__pred=(this);
		return this;
	}
}
class bb_flag_FlagType : Object{
}
class bb_tree_TreeType : Object{
}
class bb_rock_RockType : Object{
}
class bb_{
	public static int bbMain(){
		(new bb__LDApp()).g_LDApp_new();
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
		bb_gfx_GFX.g_Overlay=null;
		bb_gfx_GFX.g_Title=null;
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
		bb_entity_Entity.g_a=new bb_entity_Entity[0][];
		bb__LDApp.g_level=null;
		bb_dog_Dog.g_a=new bb_dog_Dog[0];
		bb_dog_Dog.g_gfxStandFront=null;
		bb_yeti_Yeti.g_a=new bb_yeti_Yeti[0];
		bb_yeti_Yeti.g_gfxStandFront=null;
		bb_yeti_Yeti.g_gfxRunFront=null;
		bb_yeti_Yeti.g_gfxRunLeft=null;
		bb_yeti_Yeti.g_gfxRunRight=null;
		bb_yeti_Yeti.g_gfxShadow=null;
		bb_skier_Skier.g_a=new bb_skier_Skier[0];
		bb_skier_Skier.g_gfxSki=null;
		bb_skier_Skier.g_gfxTease=null;
		bb_skier_Skier.g_gfxShadow=null;
		bb_skier_Skier.g_gfxFall=null;
		bb_tree_Tree.g_a=new bb_tree_Tree[0];
		bb_tree_Tree.g_gfxSmallTree=null;
		bb_tree_Tree.g_gfxBigTree=null;
		bb_tree_Tree.g_gfxTreeStump=null;
		bb_rock_Rock.g_a=new bb_rock_Rock[0];
		bb_rock_Rock.g_gfxRockBoulder=null;
		bb_rock_Rock.g_gfxRockSpikey=null;
		bb_rock_Rock.g_gfxRockFlat=null;
		bb_flag_Flag.g_a=new bb_flag_Flag[0];
		bb_flag_Flag.g_gfxFlag=null;
		bb_flag_Flag.g_NextFlag=0;
		bb_yeti_Yeti.g_NextYeti=0;
		bb_dog_Dog.g_NextDog=0;
		bb_skier_Skier.g_NextSkier=0;
		bb_level_Level.g_Width=1000;
		bb_random.bb_random_Seed=1234;
		bb_level_Level.g_Height=17500;
		bb_tree_Tree.g_NextTree=0;
		bb_rock_Rock.g_NextRock=0;
		bb_sfx_SFX.g_MusicActive=true;
		bb_sfx_SFX.g_CurrentMusic="";
		return 0;
	}
}
class bb_autofit{
	public static int bb_autofit_SetVirtualDisplay(int t_width,int t_height,float t_zoom){
		(new bb_autofit_VirtualDisplay()).g_VirtualDisplay_new(t_width,t_height,t_zoom);
		return 0;
	}
	public static float bb_autofit_VDeviceWidth(){
		return bb_autofit_VirtualDisplay.g_Display.f_vwidth;
	}
	public static float bb_autofit_VMouseX(bool t_limit){
		return bb_autofit_VirtualDisplay.g_Display.m_VMouseX(t_limit);
	}
	public static float bb_autofit_VDeviceHeight(){
		return bb_autofit_VirtualDisplay.g_Display.f_vheight;
	}
	public static float bb_autofit_VMouseY(bool t_limit){
		return bb_autofit_VirtualDisplay.g_Display.m_VMouseY(t_limit);
	}
	public static int bb_autofit_UpdateVirtualDisplay(bool t_zoomborders,bool t_keepborders){
		bb_autofit_VirtualDisplay.g_Display.m_UpdateVirtualDisplay(t_zoomborders,t_keepborders);
		return 0;
	}
}
class bb_controls{
}
class bb_functions{
	public static bool bb_functions_PointInRect(float t_X,float t_Y,float t_X1,float t_Y1,float t_W,float t_H){
		if(t_X<t_X1){
			return false;
		}
		if(t_Y<t_Y1){
			return false;
		}
		if(t_X>t_X1+t_W){
			return false;
		}
		if(t_Y>t_Y1+t_H){
			return false;
		}
		return true;
	}
	public static bool bb_functions_RectOverRect(float t_tX1,float t_tY1,float t_tW1,float t_tH1,float t_tX2,float t_tY2,float t_tW2,float t_tH2){
		if(t_tX2>t_tX1+t_tW1){
			return false;
		}
		if(t_tX1>t_tX2+t_tW2){
			return false;
		}
		if(t_tY2>t_tY1+t_tH1){
			return false;
		}
		if(t_tY1>t_tY2+t_tH2){
			return false;
		}
		return true;
	}
}
class bb_gfx{
	public static void bb_gfx_DrawHollowRect(int t_tX,int t_tY,int t_tW,int t_tH){
		int t_X1=t_tX;
		int t_Y1=t_tY;
		int t_X2=t_tX+t_tW;
		int t_Y2=t_tY+t_tH;
		bb_graphics.bb_graphics_DrawLine((float)(t_X1),(float)(t_Y1),(float)(t_X2),(float)(t_Y1));
		bb_graphics.bb_graphics_DrawLine((float)(t_X1),(float)(t_Y2),(float)(t_X2),(float)(t_Y2));
		bb_graphics.bb_graphics_DrawLine((float)(t_X1),(float)(t_Y1),(float)(t_X1),(float)(t_Y2));
		bb_graphics.bb_graphics_DrawLine((float)(t_X2),(float)(t_Y1),(float)(t_X2),(float)(t_Y2));
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
	public static void bb_level_GenerateFlagTrail(bb_level_Level t_tL){
		int t_tX=0;
		int t_subX=0;
		int t_tY=200;
		int t_tStep=200;
		bool t_tLeft=false;
		int t_tWidth=50;
		for(int t_i=0;t_i<150;t_i=t_i+1){
			t_tX=(int)((float)Math.Sin(((float)(t_tY))*bb_std_lang.D2R)*(float)(t_tWidth));
			t_subX=(int)((float)Math.Sin(((float)(t_tY*3))*bb_std_lang.D2R)*(float)(t_tWidth)*0.1f);
			int t_tF=bb_flag_Flag.g_Create((float)(t_tX+t_subX),(float)(t_tY));
			if(t_tLeft){
				t_tLeft=false;
				bb_flag_Flag.g_a[t_tF].f_Type=0;
			}else{
				t_tLeft=true;
				bb_flag_Flag.g_a[t_tF].f_Type=1;
			}
			t_tY+=t_tStep;
		}
	}
	public static void bb_level_GenerateTrees(bb_level_Level t_tL){
		for(int t_i=0;t_i<1000;t_i=t_i+1){
			int t_tX=(int)(bb_random.bb_random_Rnd2(0.0f-(float)(bb_level_Level.g_Width)*0.5f,(float)(bb_level_Level.g_Width)*0.5f));
			int t_tY=(int)(bb_random.bb_random_Rnd2(0.0f,(float)(bb_level_Level.g_Height)));
			bb_tree_Tree.g_Create((float)(t_tX),(float)(t_tY));
		}
	}
	public static void bb_level_GenerateRocks(bb_level_Level t_tL){
		for(int t_i=0;t_i<300;t_i=t_i+1){
			bool t_tGood=false;
			int t_tX=0;
			int t_tY=0;
			while(t_tGood==false){
				t_tX=(int)(bb_random.bb_random_Rnd2(0.0f-(float)(bb_level_Level.g_Width)*0.5f,(float)(bb_level_Level.g_Width)*0.5f));
				t_tY=(int)(bb_random.bb_random_Rnd2(0.0f,(float)(bb_level_Level.g_Height)));
				if(t_tY % 4000>2000){
					t_tGood=true;
				}
			}
			bb_rock_Rock.g_Create((float)(t_tX),(float)(t_tY));
		}
	}
	public static bb_level_Level bb_level_GenerateLevel(){
		bb_level_Level t_tL=(new bb_level_Level()).g_Level_new();
		bb_level.bb_level_GenerateFlagTrail(t_tL);
		bb_level.bb_level_GenerateTrees(t_tL);
		bb_level.bb_level_GenerateRocks(t_tL);
		return t_tL;
	}
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
		return bb_app.bb_app_device.SetUpdateRate(t_hertz);
	}
	public static String bb_app_LoadString(String t_path){
		return bb_app.bb_app_device.LoadString(bb_data.bb_data_FixDataPath(t_path));
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
		bb_audio.bb_audio_device=t_dev;
		return 0;
	}
	public static int bb_audio_MusicState(){
		return bb_audio.bb_audio_device.MusicState();
	}
	public static int bb_audio_PlayMusic(String t_path,int t_flags){
		return bb_audio.bb_audio_device.PlayMusic(bb_data.bb_data_FixDataPath(t_path),t_flags);
	}
}
class bb_audiodevice{
}
class bb_data{
	public static String bb_data_FixDataPath(String t_path){
		int t_i=t_path.IndexOf(":/",0);
		if(t_i!=-1 && t_path.IndexOf("/",0)==t_i+1){
			return t_path;
		}
		if(t_path.StartsWith("./") || t_path.StartsWith("/")){
			return t_path;
		}
		return "monkey://data/"+t_path;
	}
}
class bb_graphics{
	public static gxtkGraphics bb_graphics_device;
	public static int bb_graphics_SetGraphicsDevice(gxtkGraphics t_dev){
		bb_graphics.bb_graphics_device=t_dev;
		return 0;
	}
	public static bb_graphics_GraphicsContext bb_graphics_context;
	public static bb_graphics_Image bb_graphics_LoadImage(String t_path,int t_frameCount,int t_flags){
		gxtkSurface t_surf=bb_graphics.bb_graphics_device.LoadSurface(bb_data.bb_data_FixDataPath(t_path));
		if((t_surf)!=null){
			return ((new bb_graphics_Image()).g_Image_new()).m_Init(t_surf,t_frameCount,t_flags);
		}
		return null;
	}
	public static bb_graphics_Image bb_graphics_LoadImage2(String t_path,int t_frameWidth,int t_frameHeight,int t_frameCount,int t_flags){
		bb_graphics_Image t_atlas=bb_graphics.bb_graphics_LoadImage(t_path,1,0);
		if((t_atlas)!=null){
			return t_atlas.m_GrabImage(0,0,t_frameWidth,t_frameHeight,t_frameCount,t_flags);
		}
		return null;
	}
	public static int bb_graphics_SetFont(bb_graphics_Image t_font,int t_firstChar){
		if(!((t_font)!=null)){
			if(!((bb_graphics.bb_graphics_context.f_defaultFont)!=null)){
				bb_graphics.bb_graphics_context.f_defaultFont=bb_graphics.bb_graphics_LoadImage("mojo_font.png",96,2);
			}
			t_font=bb_graphics.bb_graphics_context.f_defaultFont;
			t_firstChar=32;
		}
		bb_graphics.bb_graphics_context.f_font=t_font;
		bb_graphics.bb_graphics_context.f_firstChar=t_firstChar;
		return 0;
	}
	public static gxtkGraphics bb_graphics_renderDevice;
	public static int bb_graphics_SetMatrix(float t_ix,float t_iy,float t_jx,float t_jy,float t_tx,float t_ty){
		bb_graphics.bb_graphics_context.f_ix=t_ix;
		bb_graphics.bb_graphics_context.f_iy=t_iy;
		bb_graphics.bb_graphics_context.f_jx=t_jx;
		bb_graphics.bb_graphics_context.f_jy=t_jy;
		bb_graphics.bb_graphics_context.f_tx=t_tx;
		bb_graphics.bb_graphics_context.f_ty=t_ty;
		bb_graphics.bb_graphics_context.f_tformed=((t_ix!=1.0f || t_iy!=0.0f || t_jx!=0.0f || t_jy!=1.0f || t_tx!=0.0f || t_ty!=0.0f)?1:0);
		bb_graphics.bb_graphics_context.f_matDirty=1;
		return 0;
	}
	public static int bb_graphics_SetMatrix2(float[] t_m){
		bb_graphics.bb_graphics_SetMatrix(t_m[0],t_m[1],t_m[2],t_m[3],t_m[4],t_m[5]);
		return 0;
	}
	public static int bb_graphics_SetColor(float t_r,float t_g,float t_b){
		bb_graphics.bb_graphics_context.f_color_r=t_r;
		bb_graphics.bb_graphics_context.f_color_g=t_g;
		bb_graphics.bb_graphics_context.f_color_b=t_b;
		bb_graphics.bb_graphics_renderDevice.SetColor(t_r,t_g,t_b);
		return 0;
	}
	public static int bb_graphics_SetAlpha(float t_alpha){
		bb_graphics.bb_graphics_context.f_alpha=t_alpha;
		bb_graphics.bb_graphics_renderDevice.SetAlpha(t_alpha);
		return 0;
	}
	public static int bb_graphics_SetBlend(int t_blend){
		bb_graphics.bb_graphics_context.f_blend=t_blend;
		bb_graphics.bb_graphics_renderDevice.SetBlend(t_blend);
		return 0;
	}
	public static int bb_graphics_DeviceWidth(){
		return bb_graphics.bb_graphics_device.Width();
	}
	public static int bb_graphics_DeviceHeight(){
		return bb_graphics.bb_graphics_device.Height();
	}
	public static int bb_graphics_SetScissor(float t_x,float t_y,float t_width,float t_height){
		bb_graphics.bb_graphics_context.f_scissor_x=t_x;
		bb_graphics.bb_graphics_context.f_scissor_y=t_y;
		bb_graphics.bb_graphics_context.f_scissor_width=t_width;
		bb_graphics.bb_graphics_context.f_scissor_height=t_height;
		bb_graphics.bb_graphics_renderDevice.SetScissor((int)(t_x),(int)(t_y),(int)(t_width),(int)(t_height));
		return 0;
	}
	public static int bb_graphics_BeginRender(){
		if(!((bb_graphics.bb_graphics_device.Mode())!=0)){
			return 0;
		}
		bb_graphics.bb_graphics_renderDevice=bb_graphics.bb_graphics_device;
		bb_graphics.bb_graphics_context.f_matrixSp=0;
		bb_graphics.bb_graphics_SetMatrix(1.0f,0.0f,0.0f,1.0f,0.0f,0.0f);
		bb_graphics.bb_graphics_SetColor(255.0f,255.0f,255.0f);
		bb_graphics.bb_graphics_SetAlpha(1.0f);
		bb_graphics.bb_graphics_SetBlend(0);
		bb_graphics.bb_graphics_SetScissor(0.0f,0.0f,(float)(bb_graphics.bb_graphics_DeviceWidth()),(float)(bb_graphics.bb_graphics_DeviceHeight()));
		return 0;
	}
	public static int bb_graphics_EndRender(){
		bb_graphics.bb_graphics_renderDevice=null;
		return 0;
	}
	public static int bb_graphics_Cls(float t_r,float t_g,float t_b){
		bb_graphics.bb_graphics_renderDevice.Cls(t_r,t_g,t_b);
		return 0;
	}
	public static int bb_graphics_Transform(float t_ix,float t_iy,float t_jx,float t_jy,float t_tx,float t_ty){
		float t_ix2=t_ix*bb_graphics.bb_graphics_context.f_ix+t_iy*bb_graphics.bb_graphics_context.f_jx;
		float t_iy2=t_ix*bb_graphics.bb_graphics_context.f_iy+t_iy*bb_graphics.bb_graphics_context.f_jy;
		float t_jx2=t_jx*bb_graphics.bb_graphics_context.f_ix+t_jy*bb_graphics.bb_graphics_context.f_jx;
		float t_jy2=t_jx*bb_graphics.bb_graphics_context.f_iy+t_jy*bb_graphics.bb_graphics_context.f_jy;
		float t_tx2=t_tx*bb_graphics.bb_graphics_context.f_ix+t_ty*bb_graphics.bb_graphics_context.f_jx+bb_graphics.bb_graphics_context.f_tx;
		float t_ty2=t_tx*bb_graphics.bb_graphics_context.f_iy+t_ty*bb_graphics.bb_graphics_context.f_jy+bb_graphics.bb_graphics_context.f_ty;
		bb_graphics.bb_graphics_SetMatrix(t_ix2,t_iy2,t_jx2,t_jy2,t_tx2,t_ty2);
		return 0;
	}
	public static int bb_graphics_Transform2(float[] t_m){
		bb_graphics.bb_graphics_Transform(t_m[0],t_m[1],t_m[2],t_m[3],t_m[4],t_m[5]);
		return 0;
	}
	public static int bb_graphics_Scale(float t_x,float t_y){
		bb_graphics.bb_graphics_Transform(t_x,0.0f,0.0f,t_y,0.0f,0.0f);
		return 0;
	}
	public static int bb_graphics_Translate(float t_x,float t_y){
		bb_graphics.bb_graphics_Transform(1.0f,0.0f,0.0f,1.0f,t_x,t_y);
		return 0;
	}
	public static int bb_graphics_DrawCircle(float t_x,float t_y,float t_r){
		bb_graphics.bb_graphics_context.m_Validate();
		bb_graphics.bb_graphics_renderDevice.DrawOval(t_x-t_r,t_y-t_r,t_r*2.0f,t_r*2.0f);
		return 0;
	}
	public static int bb_graphics_DrawRect(float t_x,float t_y,float t_w,float t_h){
		bb_graphics.bb_graphics_context.m_Validate();
		bb_graphics.bb_graphics_renderDevice.DrawRect(t_x,t_y,t_w,t_h);
		return 0;
	}
	public static int bb_graphics_DrawLine(float t_x1,float t_y1,float t_x2,float t_y2){
		bb_graphics.bb_graphics_context.m_Validate();
		bb_graphics.bb_graphics_renderDevice.DrawLine(t_x1,t_y1,t_x2,t_y2);
		return 0;
	}
	public static int bb_graphics_PushMatrix(){
		int t_sp=bb_graphics.bb_graphics_context.f_matrixSp;
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+0]=bb_graphics.bb_graphics_context.f_ix;
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+1]=bb_graphics.bb_graphics_context.f_iy;
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+2]=bb_graphics.bb_graphics_context.f_jx;
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+3]=bb_graphics.bb_graphics_context.f_jy;
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+4]=bb_graphics.bb_graphics_context.f_tx;
		bb_graphics.bb_graphics_context.f_matrixStack[t_sp+5]=bb_graphics.bb_graphics_context.f_ty;
		bb_graphics.bb_graphics_context.f_matrixSp=t_sp+6;
		return 0;
	}
	public static int bb_graphics_PopMatrix(){
		int t_sp=bb_graphics.bb_graphics_context.f_matrixSp-6;
		bb_graphics.bb_graphics_SetMatrix(bb_graphics.bb_graphics_context.f_matrixStack[t_sp+0],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+1],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+2],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+3],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+4],bb_graphics.bb_graphics_context.f_matrixStack[t_sp+5]);
		bb_graphics.bb_graphics_context.f_matrixSp=t_sp;
		return 0;
	}
	public static int bb_graphics_DrawImage(bb_graphics_Image t_image,float t_x,float t_y,int t_frame){
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		if((bb_graphics.bb_graphics_context.f_tformed)!=0){
			bb_graphics.bb_graphics_PushMatrix();
			bb_graphics.bb_graphics_Translate(t_x-t_image.f_tx,t_y-t_image.f_ty);
			bb_graphics.bb_graphics_context.m_Validate();
			if((t_image.f_flags&65536)!=0){
				bb_graphics.bb_graphics_renderDevice.DrawSurface(t_image.f_surface,0.0f,0.0f);
			}else{
				bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
			}
			bb_graphics.bb_graphics_PopMatrix();
		}else{
			bb_graphics.bb_graphics_context.m_Validate();
			if((t_image.f_flags&65536)!=0){
				bb_graphics.bb_graphics_renderDevice.DrawSurface(t_image.f_surface,t_x-t_image.f_tx,t_y-t_image.f_ty);
			}else{
				bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,t_x-t_image.f_tx,t_y-t_image.f_ty,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
			}
		}
		return 0;
	}
	public static int bb_graphics_Rotate(float t_angle){
		bb_graphics.bb_graphics_Transform((float)Math.Cos((t_angle)*bb_std_lang.D2R),-(float)Math.Sin((t_angle)*bb_std_lang.D2R),(float)Math.Sin((t_angle)*bb_std_lang.D2R),(float)Math.Cos((t_angle)*bb_std_lang.D2R),0.0f,0.0f);
		return 0;
	}
	public static int bb_graphics_DrawImage2(bb_graphics_Image t_image,float t_x,float t_y,float t_rotation,float t_scaleX,float t_scaleY,int t_frame){
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		bb_graphics.bb_graphics_PushMatrix();
		bb_graphics.bb_graphics_Translate(t_x,t_y);
		bb_graphics.bb_graphics_Rotate(t_rotation);
		bb_graphics.bb_graphics_Scale(t_scaleX,t_scaleY);
		bb_graphics.bb_graphics_Translate(-t_image.f_tx,-t_image.f_ty);
		bb_graphics.bb_graphics_context.m_Validate();
		if((t_image.f_flags&65536)!=0){
			bb_graphics.bb_graphics_renderDevice.DrawSurface(t_image.f_surface,0.0f,0.0f);
		}else{
			bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
		}
		bb_graphics.bb_graphics_PopMatrix();
		return 0;
	}
	public static int bb_graphics_DrawImageRect(bb_graphics_Image t_image,float t_x,float t_y,int t_srcX,int t_srcY,int t_srcWidth,int t_srcHeight,int t_frame){
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		if((bb_graphics.bb_graphics_context.f_tformed)!=0){
			bb_graphics.bb_graphics_PushMatrix();
			bb_graphics.bb_graphics_Translate(-t_image.f_tx+t_x,-t_image.f_ty+t_y);
			bb_graphics.bb_graphics_context.m_Validate();
			bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
			bb_graphics.bb_graphics_PopMatrix();
		}else{
			bb_graphics.bb_graphics_context.m_Validate();
			bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,-t_image.f_tx+t_x,-t_image.f_ty+t_y,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
		}
		return 0;
	}
	public static int bb_graphics_DrawImageRect2(bb_graphics_Image t_image,float t_x,float t_y,int t_srcX,int t_srcY,int t_srcWidth,int t_srcHeight,float t_rotation,float t_scaleX,float t_scaleY,int t_frame){
		bb_graphics_Frame t_f=t_image.f_frames[t_frame];
		bb_graphics.bb_graphics_PushMatrix();
		bb_graphics.bb_graphics_Translate(t_x,t_y);
		bb_graphics.bb_graphics_Rotate(t_rotation);
		bb_graphics.bb_graphics_Scale(t_scaleX,t_scaleY);
		bb_graphics.bb_graphics_Translate(-t_image.f_tx,-t_image.f_ty);
		bb_graphics.bb_graphics_context.m_Validate();
		bb_graphics.bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0f,0.0f,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
		bb_graphics.bb_graphics_PopMatrix();
		return 0;
	}
	public static int bb_graphics_DrawText(String t_text,float t_x,float t_y,float t_xalign,float t_yalign){
		if(!((bb_graphics.bb_graphics_context.f_font)!=null)){
			return 0;
		}
		int t_w=bb_graphics.bb_graphics_context.f_font.m_Width();
		int t_h=bb_graphics.bb_graphics_context.f_font.m_Height();
		t_x-=(float)Math.Floor((float)(t_w*t_text.Length)*t_xalign);
		t_y-=(float)Math.Floor((float)(t_h)*t_yalign);
		for(int t_i=0;t_i<t_text.Length;t_i=t_i+1){
			int t_ch=(int)t_text[t_i]-bb_graphics.bb_graphics_context.f_firstChar;
			if(t_ch>=0 && t_ch<bb_graphics.bb_graphics_context.f_font.m_Frames()){
				bb_graphics.bb_graphics_DrawImage(bb_graphics.bb_graphics_context.f_font,t_x+(float)(t_i*t_w),t_y,t_ch);
			}
		}
		return 0;
	}
}
class bb_graphicsdevice{
}
class bb_input{
	public static gxtkInput bb_input_device;
	public static int bb_input_SetInputDevice(gxtkInput t_dev){
		bb_input.bb_input_device=t_dev;
		return 0;
	}
	public static int bb_input_KeyDown(int t_key){
		return bb_input.bb_input_device.KeyDown(t_key);
	}
	public static float bb_input_JoyX(int t_index,int t_unit){
		return bb_input.bb_input_device.JoyX(t_index|t_unit<<4);
	}
	public static float bb_input_JoyY(int t_index,int t_unit){
		return bb_input.bb_input_device.JoyY(t_index|t_unit<<4);
	}
	public static int bb_input_JoyDown(int t_button,int t_unit){
		return bb_input.bb_input_device.KeyDown(t_button|t_unit<<4|256);
	}
	public static int bb_input_MouseHit(int t_button){
		return bb_input.bb_input_device.KeyHit(1+t_button);
	}
	public static float bb_input_MouseX(){
		return bb_input.bb_input_device.MouseX();
	}
	public static float bb_input_MouseY(){
		return bb_input.bb_input_device.MouseY();
	}
	public static int bb_input_MouseDown(int t_button){
		return bb_input.bb_input_device.KeyDown(1+t_button);
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
		if(t_n<t_min){
			return t_min;
		}
		if(t_n>t_max){
			return t_max;
		}
		return t_n;
	}
	public static float bb_math_Clamp2(float t_n,float t_min,float t_max){
		if(t_n<t_min){
			return t_min;
		}
		if(t_n>t_max){
			return t_max;
		}
		return t_n;
	}
	public static int bb_math_Max(int t_x,int t_y){
		if(t_x>t_y){
			return t_x;
		}
		return t_y;
	}
	public static float bb_math_Max2(float t_x,float t_y){
		if(t_x>t_y){
			return t_x;
		}
		return t_y;
	}
	public static int bb_math_Min(int t_x,int t_y){
		if(t_x<t_y){
			return t_x;
		}
		return t_y;
	}
	public static float bb_math_Min2(float t_x,float t_y){
		if(t_x<t_y){
			return t_x;
		}
		return t_y;
	}
	public static int bb_math_Abs(int t_x){
		if(t_x>=0){
			return t_x;
		}
		return -t_x;
	}
	public static float bb_math_Abs2(float t_x){
		if(t_x>=0.0f){
			return t_x;
		}
		return -t_x;
	}
}
class bb_monkey{
}
class bb_random{
	public static int bb_random_Seed;
	public static float bb_random_Rnd(){
		bb_random.bb_random_Seed=bb_random.bb_random_Seed*1664525+1013904223|0;
		return (float)(bb_random.bb_random_Seed>>8&16777215)/16777216.0f;
	}
	public static float bb_random_Rnd2(float t_low,float t_high){
		return bb_random.bb_random_Rnd3(t_high-t_low)+t_low;
	}
	public static float bb_random_Rnd3(float t_range){
		return bb_random.bb_random_Rnd()*t_range;
	}
}
class bb_set{
}
class bb_stack{
}
//${TRANSCODE_END}
