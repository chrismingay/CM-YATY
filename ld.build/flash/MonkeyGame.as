
package{

	import flash.display.*;
	import flash.events.*;
	import flash.media.*;
	import flash.net.*;
	import flash.utils.ByteArray;

	[SWF(width="640",height="480")]
	[Frame(factoryClass="Preloader")]
	
	public class MonkeyGame extends Sprite{
	
		public var runner:Function;
		
		public function MonkeyGame(){
		
			game=this;

			addEventListener( Event.ADDED_TO_STAGE,onAddedToStage );
		}
		
		private function onAddedToStage( e:Event ):void{
		
			try{
				bbInit();
				bbMain();

				if( runner!=null ) runner();

			}catch( err:Object ){
			
				printError( err.toString() );
			}
		}
		
		private function mungPath( path:String ):String{
		
			if( path.toLowerCase().indexOf("monkey://data/")!=0 ) return "";
			path=path.slice(14);
			
			var i:int=path.indexOf( "." ),ext:String="";
			if( i!=-1 ){
				ext=path.slice(i+1);
				path=path.slice(0,i);
			}

			var munged:String="_";
			var bits:Array=path.split( "/" );
			
			for( i=0;i<bits.length;++i ){
				munged+=bits[i].length+bits[i];
			}
			munged+=ext.length+ext;
			
			return munged;
		}
		
		public function urlRequest( path:String ):URLRequest{
			if( path.toLowerCase().indexOf("monkey://data/")==0 ) path="data/"+path.slice(14);
			return new URLRequest( path );
		}
		
		public function loadByteArray( path:String ):ByteArray{
			path=mungPath( path );
			var t:Class=Assets[path];
			if( t ) return (new t) as ByteArray;
			return null;
		}
		
		public function loadString( path:String ):String{
			var buf:ByteArray=loadByteArray( path );
			if( buf ) return buf.toString();
			return "";
		}

		public function loadBitmap( path:String ):Bitmap{
			path=mungPath( path );
			var t:Class=Assets[path];
			if( t ) return (new t) as Bitmap;
			return null;
		}
		
		public function loadSound( path:String ):Sound{
			path=mungPath( path );
			var t:Class=Assets[path];
			if( t ) return (new t) as Sound;
			return null;
		}
	}		
}

var game:MonkeyGame;

final class Config{
//${CONFIG_BEGIN}
internal static var BINARY_FILES:String="*.bin|*.dat"
internal static var CD:String=""
internal static var CONFIG:String="release"
internal static var HOST:String="winnt"
internal static var IMAGE_FILES:String="*.png|*.jpg"
internal static var LANG:String="as"
internal static var MODPATH:String=".;C:/Users/Chris/Documents/GitHub/CM-YATY;C:/apps/MonkeyPro66/modules"
internal static var MOJO_AUTO_SUSPEND_ENABLED:String="0"
internal static var MOJO_IMAGE_FILTERING_ENABLED:String="0"
internal static var MUSIC_FILES:String="*.mp3"
internal static var SAFEMODE:String="0"
internal static var SOUND_FILES:String="*.mp3"
internal static var TARGET:String="flash"
internal static var TEXT_FILES:String="*.txt|*.xml|*.json"
internal static var TRANSDIR:String=""
internal static var XNA_WINDOW_HEIGHT:String="720"
internal static var XNA_WINDOW_WIDTH:String="480"
//${CONFIG_END}
}

final class Assets{
//${ASSETS_BEGIN}
[Embed(source="data/gfx/fonts.png")]
public static var _3gfx5fonts3png:Class;
[Embed(source="data/gfx/overlay.png")]
public static var _3gfx7overlay3png:Class;
[Embed(source="data/gfx/sheet.png")]
public static var _3gfx5sheet3png:Class;
[Embed(source="data/gfx/title.png")]
public static var _3gfx5title3png:Class;
[Embed(source="data/mus/ambient.mp3")]
public static var _3mus7ambient3mp3:Class;
[Embed(source="data/mus/chase.mp3")]
public static var _3mus5chase3mp3:Class;
[Embed(source="data/txt/wait.txt",mimeType="application/octet-stream")]
public static var _3txt4wait3txt:Class;
[Embed(source="data/mojo_font.png")]
public static var _9mojo_font3png:Class;
//${ASSETS_END}
}

//${TRANSCODE_BEGIN}

// Actionscript Monkey runtime.
//
// Placed into the public domain 24/02/2011.
// No warranty implied; use at your own risk.

//***** ActionScript Runtime *****

import flash.display.*;
import flash.text.*;
import flash.external.ExternalInterface;

//Consts for radians<->degrees conversions
var D2R:Number=0.017453292519943295;
var R2D:Number=57.29577951308232;

//private
var _console:TextField;
var _errInfo:String="?<?>";
var _errStack:Array=[];

var dbg_index:int=0;

function _getConsole():TextField{
	if( _console ) return _console;
	_console=new TextField();
	_console.x=0;
	_console.y=0;
	_console.width=game.stage.stageWidth;
	_console.height=game.stage.stageHeight;
	_console.background=false;
	_console.backgroundColor=0xff000000;
	_console.textColor=0xffffff00;
	game.stage.addChild( _console );
	return _console;
}

function pushErr():void{
	_errStack.push( _errInfo );
}

function popErr():void{
	_errInfo=_errStack.pop();
}

function stackTrace():String{
	if( !_errInfo.length ) return "";
	var str:String=_errInfo+"\n";
	for( var i:int=_errStack.length-1;i>0;--i ){
		str+=_errStack[i]+"\n";
	}
	return str;
}

function print( str:String ):int{
	var console:TextField=_getConsole();
	if( !console ) return 0;
	console.appendText( str+"\n" );
	return 0;
}

function printError( err:Object ):void{
	var msg:String=err.toString();
	if( !msg.length ) return;
	print( "Monkey Runtime Error : "+msg );
	print( "" );
	print( stackTrace() );
}

function error( err:String ):int{
	throw err;
}

function debugLog( str:String ):int{
	print( str );
	return 0;
}

function debugStop():int{
	error( "STOP" );
	return 0;
}

function dbg_object( obj:Object ):Object{
	if( obj ) return obj;
	error( "Null object access" );
	return obj;
}

function dbg_array( arr:Array,index:int ):Array{
	if( index<0 || index>=arr.length ) error( "Array index out of range" );
	dbg_index=index;
	return arr;
}

function new_bool_array( len:int ):Array{
	var arr:Array=new Array( len )
	for( var i:int=0;i<len;++i ) arr[i]=false;
	return arr;
}

function new_number_array( len:int ):Array{
	var arr:Array=new Array( len )
	for( var i:int=0;i<len;++i ) arr[i]=0;
	return arr;
}

function new_string_array( len:int ):Array{
	var arr:Array=new Array( len );
	for( var i:int=0;i<len;++i ) arr[i]='';
	return arr;
}

function new_array_array( len:int ):Array{
	var arr:Array=new Array( len );
	for( var i:int=0;i<len;++i ) arr[i]=[];
	return arr;
}

function new_object_array( len:int ):Array{
	var arr:Array=new Array( len );
	for( var i:int=0;i<len;++i ) arr[i]=null;
	return arr;
}

function resize_bool_array( arr:Array,len:int ):Array{
	var i:int=arr.length;
	arr=arr.slice(0,len);
	if( len<=i ) return arr;
	arr.length=len;
	while( i<len ) arr[i++]=false;
	return arr;
}

function resize_number_array( arr:Array,len:int ):Array{
	var i:int=arr.length;
	arr=arr.slice(0,len);
	if( len<=i ) return arr;
	arr.length=len;
	while( i<len ) arr[i++]=0;
	return arr;
}

function resize_string_array( arr:Array,len:int ):Array{
	var i:int=arr.length;
	arr=arr.slice(0,len);
	if( len<=i ) return arr;
	arr.length=len;
	while( i<len ) arr[i++]="";
	return arr;
}

function resize_array_array( arr:Array,len:int ):Array{
	var i:int=arr.length;
	arr=arr.slice(0,len);
	if( len<=i ) return arr;
	arr.length=len;
	while( i<len ) arr[i++]=[];
	return arr;
}

function resize_object_array( arr:Array,len:int ):Array{
	var i:int=arr.length;
	arr=arr.slice(0,len);
	if( len<=i ) return arr;
	arr.length=len;
	while( i<len ) arr[i++]=null;
	return arr;
}

function string_compare( lhs:String,rhs:String ):int{
	var n:int=Math.min( lhs.length,rhs.length ),i:int,t:int;
	for( i=0;i<n;++i ){
		t=lhs.charCodeAt(i)-rhs.charCodeAt(i);
		if( t ) return t;
	}
	return lhs.length-rhs.length;
}

function string_replace( str:String,find:String,rep:String ):String{	//no unregex replace all?!?
	var i:int=0;
	for(;;){
		i=str.indexOf( find,i );
		if( i==-1 ) return str;
		str=str.substring( 0,i )+rep+str.substring( i+find.length );
		i+=rep.length;
	}
	return str;
}

function string_trim( str:String ):String{
	var i:int=0,i2:int=str.length;
	while( i<i2 && str.charCodeAt(i)<=32 ) i+=1;
	while( i2>i && str.charCodeAt(i2-1)<=32 ) i2-=1;
	return str.slice( i,i2 );
}

function string_tochars( str:String ):Array{
	var arr:Array=new Array( str.length );
	for( var i:int=0;i<str.length;++i ) arr[i]=str.charCodeAt(i);
	return arr;	
}

function string_startswith( str:String,sub:String ):Boolean{
	return sub.length<=str.length && str.slice(0,sub.length)==sub;
}

function string_endswith( str:String,sub:String ):Boolean{
	return sub.length<=str.length && str.slice(str.length-sub.length,str.length)==sub;
}

function string_fromchars( chars:Array ):String{
	var str:String="",i:int;
	for( i=0;i<chars.length;++i ){
		str+=String.fromCharCode( chars[i] );
	}
	return str;
}

class ThrowableObject{
	internal function toString():String{
		return "Uncaught Monkey Exception";
	}
}

class BBDataBuffer{

	internal var _data:ByteArray=null;
	internal var _length:int=0;
	
	public function _Init( data:ByteArray ):void{
		_data=data;
		_length=data.length;
	}
	
	public function _New( length:int ):Boolean{
		if( _data ) return false
		_data=new ByteArray;
		_data.length=length;
		_length=length;
		return true;
	}
	
	public function _Load( path:String ):Boolean{
		if( _data ) return false
		var data:ByteArray=game.loadByteArray( path );
		if( !data ) return false;
		_Init( data );
		return true;
	}
	
	public function Discard():void{
		if( _data ){
			_data.clear();
			_data=null;
			_length=0;
		}
	}
	
	public function Length():int{
		return _length;
	}
	
	public function PokeByte( addr:int,value:int ):void{
		_data.position=addr;
		_data.writeByte( value );
	}
	
	public function PokeShort( addr:int,value:int ):void{
		_data.position=addr;
		_data.writeShort( value );
	}
	
	public function PokeInt( addr:int,value:int ):void{
		_data.position=addr;
		_data.writeInt( value );
	}
	
	public function PokeFloat( addr:int,value:Number ):void{
		_data.position=addr;
		_data.writeFloat( value );
	}
	
	public function PeekByte( addr:int ):int{
		_data.position=addr;
		return _data.readByte();
	}
	
	public function PeekShort( addr:int ):int{
		_data.position=addr;
		return _data.readShort();
	}

	public function PeekInt( addr:int ):int{
		_data.position=addr;
		return _data.readInt();
	}
	
	public function PeekFloat( addr:int ):Number{
		_data.position=addr;
		return _data.readFloat();
	}
}

// Flash mojo runtime.
//
// Copyright 2011 Mark Sibly, all rights reserved.
// No warranty implied; use at your own risk.

import flash.display.*;
import flash.events.*;
import flash.media.*;
import flash.geom.*;
import flash.utils.*;
import flash.net.*;

var app:gxtkApp;

class gxtkApp{

	internal var graphics:gxtkGraphics;
	internal var input:gxtkInput;
	internal var audio:gxtkAudio;

	internal var dead:int=0;
	internal var suspended:int=0;
	internal var loading:int=0;
	internal var maxloading:int=0;
	internal var updateRate:int=0;
	internal var nextUpdate:Number=0;
	internal var updatePeriod:Number=0;
	internal var startMillis:Number=0;
	
	function gxtkApp(){
		app=this;
		
		graphics=new gxtkGraphics;
		input=new gxtkInput;
		audio=new gxtkAudio;
		
		startMillis=(new Date).getTime();
		
		game.stage.addEventListener( Event.ACTIVATE,OnActivate );
		game.stage.addEventListener( Event.DEACTIVATE,OnDeactivate );
		game.stage.addEventListener( Event.ENTER_FRAME,OnEnterFrame );
		
		SetFrameRate( 0 );
		
		game.runner=function():void{
			InvokeOnCreate();
			InvokeOnRender();
		};
	}

	internal function IncLoading():void{
		++loading;
		if( loading>maxloading ) maxloading=loading;
		if( loading!=1 ) return;
		if( updateRate ) SetFrameRate( 0 );
	}

	internal function DecLoading():void{
		--loading;
		if( loading ) return;
		maxloading=0;
		if( updateRate ) SetFrameRate( updateRate );
	}
	
	internal function SetFrameRate( fps:int ):void{
		if( fps ){
			updatePeriod=1000.0/fps;
			nextUpdate=(new Date).getTime()+updatePeriod;
			game.stage.frameRate=fps;
		}else{
			updatePeriod=0;
			game.stage.frameRate=24;
		}
	}
	
	internal function OnActivate( e:Event ):void{
		if( Config.MOJO_AUTO_SUSPEND_ENABLED=="1" ){
			InvokeOnResume();
		}
	}
	
	internal function OnDeactivate( e:Event ):void{
		if( Config.MOJO_AUTO_SUSPEND_ENABLED=="1" ){
			InvokeOnSuspend();
		}
	}
	
	internal function OnEnterFrame( e:Event ):void{
		if( !updatePeriod ) return;
		
		var updates:int=0;

		for(;;){
			nextUpdate+=updatePeriod;
			InvokeOnUpdate();
			if( !updatePeriod ) break;
			
			if( nextUpdate>(new Date).getTime() ) break;
			
			if( ++updates==7 ){
				nextUpdate=(new Date).getTime();
				break;
			}
		}
		InvokeOnRender();
	}
	
	internal function Die( err:Object ):void{
		dead=1;
		audio.OnSuspend();
		printError( err );
	}
	
	internal function InvokeOnCreate():void{
		if( dead ) return;
		
		try{
			dead=1;
			OnCreate();
			dead=0;
		}catch( err:Object ){
			Die( err );
		}
	}

	internal function InvokeOnUpdate():void{
		if( dead || suspended || !updateRate || loading ) return;
		
		try{
			input.BeginUpdate();
			OnUpdate();
			input.EndUpdate();
		}catch( err:Object ){
			Die( err );
		}
	}

	internal function InvokeOnRender():void{
		if( dead || suspended ) return;
		
		try{
			graphics.BeginRender();
			if( loading ){
				OnLoading();
			}else{
				OnRender();
			}
			graphics.EndRender();
		}catch( err:Object ){
			Die( err );
		}
	}
	
	internal function InvokeOnSuspend():void{
		if( dead || suspended ) return;
		
		try{
			suspended=1;
			OnSuspend();
			audio.OnSuspend();
		}catch( err:Object ){
			Die( err );
		}
	}
	
	internal function InvokeOnResume():void{
		if( dead || !suspended ) return;
		
		try{
			audio.OnResume();
			OnResume();
			suspended=0;
		}catch( err:Object ){
			Die( err );
		}
	}
	
	//***** GXTK API *****
	
	public function GraphicsDevice():gxtkGraphics{
		return graphics;
	}

	public function InputDevice():gxtkInput{
		return input;
	}

	public function AudioDevice():gxtkAudio{
		return audio;
	}

	public function AppTitle():String{
		return graphics.bitmap.loaderInfo.url;
	}
	
	public function LoadState():String{
		var file:SharedObject=SharedObject.getLocal( "gxtkapp" );
		var state:String=file.data.state;
		file.close();
		if( state ) return state;
		return "";
	}
	
	public function SaveState( state:String ):int{
		var file:SharedObject=SharedObject.getLocal( "gxtkapp" );
		file.data.state=state;
		file.close();
		return 0;
	}
	
	public function LoadString( path:String ):String{
		return game.loadString( path );
	}

	public function SetUpdateRate( hertz:int ):int{
		updateRate=hertz;

		if( !loading ) SetFrameRate( updateRate );

		return 0;
	}
	
	public function MilliSecs():int{
		return (new Date).getTime()-startMillis;
	}

	public function Loading():int{
		return loading;
	}

	public function OnCreate():int{
		return 0;
	}

	public function OnUpdate():int{
		return 0;
	}
	
	public function OnSuspend():int{
		return 0;
	}
	
	public function OnResume():int{
		return 0;
	}
	
	public function OnRender():int{
		return 0;
	}
	
	public function OnLoading():int{
		return 0;
	}

}

class gxtkGraphics{
	internal var bitmap:Bitmap;
	
	internal var red:Number=255;
	internal var green:Number=255;
	internal var blue:Number=255;
	internal var alpha:Number=1;
	internal var colorARGB:uint=0xffffffff;
	internal var colorTform:ColorTransform=null;
	internal var alphaTform:ColorTransform=null;
	
	internal var matrix:Matrix;
	internal var rectBMData:BitmapData;
	internal var blend:String;
	internal var clipRect:Rectangle;
	
	internal var shape:Shape;
	internal var graphics:Graphics;
	internal var bitmapData:BitmapData;

	internal var pointMat:Matrix=new Matrix;
	internal var rectMat:Matrix=new Matrix;
	
	internal var image_filtering_enabled:Boolean;
	
	function gxtkGraphics(){
	
		var stage:Stage=game.stage;
	
		bitmap=new Bitmap();
		bitmap.bitmapData=new BitmapData( stage.stageWidth,stage.stageHeight,false,0xff0000ff );
		bitmap.width=stage.stageWidth;
		bitmap.height=stage.stageHeight;
		game.addChild( bitmap );

		stage.addEventListener( Event.RESIZE,OnResize );
	
		rectBMData=new BitmapData( 1,1,false,0xffffffff );
		
		image_filtering_enabled=(Config.MOJO_IMAGE_FILTERING_ENABLED=="1");
	}
	
	internal function OnResize( e:Event ):void{
		var stage:Stage=game.stage;
		var w:int=stage.stageWidth;
		var h:int=stage.stageHeight;
		if( w==bitmap.width && h==bitmap.height ) return;
		bitmap.bitmapData=new BitmapData( w,h,false,0xff0000ff );
		bitmap.width=w;
		bitmap.height=h;
	}

	internal function BeginRender():void{
		bitmapData=bitmap.bitmapData;
	}

	internal function UseBitmap():void{
		if( graphics==null ) return;
		bitmapData.draw( shape,matrix,alphaTform,blend,clipRect,false );
		graphics.clear();
		graphics=null;
	}

	internal function UseGraphics():void{
		if( graphics!=null ) return;
		if( shape==null ) shape=new Shape;
		graphics=shape.graphics;
	}

	internal function FlushGraphics():void{
		if( graphics==null ) return;
		bitmapData.draw( shape,matrix,alphaTform,blend,clipRect,false );
		graphics.clear();
	}
	
	internal function EndRender():void{
		UseBitmap();
		bitmapData=null;
	}
	
	internal function updateColor():void{
	
		colorARGB=(int(alpha*255)<<24)|(int(red)<<16)|(int(green)<<8)|int(blue);
		
		if( colorARGB==0xffffffff ){
			colorTform=null;
			alphaTform=null;
		}else{
			colorTform=new ColorTransform( red/255.0,green/255.0,blue/255.0,alpha );
			if( alpha==1 ){
				alphaTform=null;
			}else{
				alphaTform=new ColorTransform( 1,1,1,alpha );
			}
		}
	}

	//***** GXTK API *****

	public function Mode():int{
		return 1;
	}
	
	public function Width():int{
		return bitmap.width;
	}

	public function Height():int{
		return bitmap.height;
	}

	public function LoadSurface( path:String ):gxtkSurface{
		var bitmap:Bitmap=game.loadBitmap( path );
		if( bitmap==null ) return null;
		return new gxtkSurface( bitmap );
	}
	
	public function CreateSurface( width:int,height:int ):gxtkSurface{
		var bitmapData:BitmapData=new BitmapData( width,height );
		var bitmap:Bitmap=new Bitmap( bitmapData );
		return new gxtkSurface( bitmap );
	}
	
	public function SetAlpha( a:Number ):int{
		FlushGraphics();
		
		alpha=a;
		
		updateColor();
		
		return 0;
	}
	
	public function SetColor( r:Number,g:Number,b:Number ):int{
		FlushGraphics();
		
		red=r;
		green=g;
		blue=b;
		
		updateColor();
		
		return 0;
	}
	
	public function SetBlend( blend:int ):int{
		switch( blend ){
		case 1:
			this.blend=BlendMode.ADD;
			break;
		default:
			this.blend=null;
		}
		return 0;
	}
	
	public function SetScissor( x:int,y:int,w:int,h:int ):int{
		FlushGraphics();
		
		if( x!=0 || y!=0 || w!=bitmap.width || h!=bitmap.height ){
			clipRect=new Rectangle( x,y,w,h );
		}else{
			clipRect=null;
		}
		return 0;
	}

	public function SetMatrix( ix:Number,iy:Number,jx:Number,jy:Number,tx:Number,ty:Number ):int{
		FlushGraphics();
		
		if( ix!=1 || iy!=0 || jx!=0 || jy!=1 || tx!=0 || ty!=0 ){
			matrix=new Matrix( ix,iy,jx,jy,tx,ty );
		}else{
			matrix=null;
		}
		return 0;
	}

	public function Cls( r:Number,g:Number,b:Number ):int{
		UseBitmap();

		var clsColor:uint=0xff000000|(int(r)<<16)|(int(g)<<8)|int(b);
		var rect:Rectangle=clipRect;
		if( !rect ) rect=new Rectangle( 0,0,bitmap.width,bitmap.height );
		bitmapData.fillRect( rect,clsColor );
		return 0;
	}
	
	public function DrawPoint( x:Number,y:Number ):int{
		UseBitmap();
		
		if( matrix ){
			var px:Number=x;
			x=px * matrix.a + y * matrix.c + matrix.tx;
			y=px * matrix.b + y * matrix.d + matrix.ty;
		}
		if( clipRect || alphaTform || blend ){
			pointMat.tx=x;pointMat.ty=y;
			bitmapData.draw( rectBMData,pointMat,colorTform,blend,clipRect,false );
		}else{
			bitmapData.fillRect( new Rectangle( x,y,1,1 ),colorARGB );
		}
		return 0;
	}
	
	
	public function DrawRect( x:Number,y:Number,w:Number,h:Number ):int{
		UseBitmap();

		if( matrix ){
			var mat:Matrix=new Matrix( w,0,0,h,x,y );
			mat.concat( matrix );
			bitmapData.draw( rectBMData,mat,colorTform,blend,clipRect,false );
		}else if( clipRect || alphaTform || blend ){
			rectMat.a=w;rectMat.d=h;rectMat.tx=x;rectMat.ty=y;
			bitmapData.draw( rectBMData,rectMat,colorTform,blend,clipRect,false );
		}else{
			bitmapData.fillRect( new Rectangle( x,y,w,h ),colorARGB );
		}
		return 0;
	}

	public function DrawLine( x1:Number,y1:Number,x2:Number,y2:Number ):int{
		UseGraphics();
		
		if( matrix ){

			var x1_t:Number=x1 * matrix.a + y1 * matrix.c + matrix.tx;
			var y1_t:Number=x1 * matrix.b + y1 * matrix.d + matrix.ty;
			var x2_t:Number=x2 * matrix.a + y2 * matrix.c + matrix.tx;
			var y2_t:Number=x2 * matrix.b + y2 * matrix.d + matrix.ty;
			
			graphics.lineStyle( 1,colorARGB & 0xffffff );	//why the mask?
			graphics.moveTo( x1_t,y1_t );
			graphics.lineTo( x2_t,y2_t );
			graphics.lineStyle();
			
			var mat:Matrix=matrix;matrix=null;

			FlushGraphics();

			matrix=mat;
			
		}else{

			graphics.lineStyle( 1,colorARGB & 0xffffff );	//why the mask?
			graphics.moveTo( x1,y1 );
			graphics.lineTo( x2,y2 );
			graphics.lineStyle();
		
			if( alphaTform ) FlushGraphics();
		}

		return 0;
 	}

	public function DrawOval( x:Number,y:Number,w:Number,h:Number ):int{
		UseGraphics();

		graphics.beginFill( colorARGB & 0xffffff );			//why the mask?
		graphics.drawEllipse( x,y,w,h );
		graphics.endFill();
		
		if( alphaTform ) FlushGraphics();

		return 0;
	}
	
	public function DrawPoly( verts:Array ):int{
		if( verts.length<6 ) return 0;
		
		UseGraphics();
		
		graphics.beginFill( colorARGB & 0xffffff );			//why the mask?
		
		graphics.moveTo( verts[0],verts[1] );
		for( var i:int=0;i<verts.length;i+=2 ){
			graphics.lineTo( verts[i],verts[i+1] );
		}
		graphics.endFill();
		
		if( alphaTform ) FlushGraphics();

		return 0;
	}

	public function DrawSurface( surface:gxtkSurface,x:Number,y:Number ):int{
		UseBitmap();

		if( matrix ){
			if( x!=0 || y!=0 ){
				//have to translate matrix! TODO!
				return -1;
			}
			bitmapData.draw( surface.bitmap.bitmapData,matrix,colorTform,blend,clipRect,image_filtering_enabled );
		}else if( clipRect || colorTform || blend ){
			var mat:Matrix=new Matrix( 1,0,0,1,x,y );
			bitmapData.draw( surface.bitmap.bitmapData,mat,colorTform,blend,clipRect,image_filtering_enabled );
		}else{
			bitmapData.copyPixels( surface.bitmap.bitmapData,surface.rect,new Point( x,y ) );
		}
		return 0;
	}

	public function DrawSurface2( surface:gxtkSurface,x:Number,y:Number,srcx:int,srcy:int,srcw:int,srch:int ):int{
		if( srcw<0 ){ srcx+=srcw;srcw=-srcw; }
		if( srch<0 ){ srcy+=srch;srch=-srch; }
		if( srcw<=0 || srch<=0 ) return 0;
		
		UseBitmap();

		var srcrect:Rectangle=new Rectangle( srcx,srcy,srcw,srch );
		
		if( matrix || clipRect || colorTform || blend ){

			var scratch:BitmapData=surface.scratch;
			if( scratch==null || srcw!=scratch.width || srch!=scratch.height ){
				if( scratch!=null ) scratch.dispose();
				scratch=new BitmapData( srcw,srch );
				surface.scratch=scratch;
			}
			scratch.copyPixels( surface.bitmap.bitmapData,srcrect,new Point( 0,0 ) );
			
			var mmatrix:Matrix=matrix;
			if( mmatrix==null ){
				mmatrix=new Matrix( 1,0,0,1,x,y );
			}else if( x!=0 || y!=0 ){
				//have to translate matrix! TODO!
				return -1;
			}

			bitmapData.draw( scratch,mmatrix,colorTform,blend,clipRect,image_filtering_enabled );
		}else{
			bitmapData.copyPixels( surface.bitmap.bitmapData,srcrect,new Point( x,y ) );
		}
		return 0;
	}
	
	public function ReadPixels( pixels:Array,x:int,y:int,width:int,height:int,offset:int,pitch:int ):int{
	
		UseBitmap();
		
		var data:ByteArray=bitmapData.getPixels( new Rectangle( x,y,width,height ) );
		data.position=0;
		
		var px:int,py:int,j:int=offset,argb:int;
		
		for( py=0;py<height;++py ){
			for( px=0;px<width;++px ){
				pixels[j++]=data.readInt();
			}
			j+=pitch-width;
		}
		
		return 0;
	}
	
	public function WritePixels2( surface:gxtkSurface,pixels:Array,x:int,y:int,width:int,height:int,offset:int,pitch:int ):int{

		UseBitmap();
		
		var data:ByteArray=new ByteArray();
		data.length=width*height;
			
		var px:int,py:int,j:int=offset,argb:int;
		
		for( py=0;py<height;++py ){
			for( px=0;px<width;++px ){
				data.writeInt( pixels[j++] );
			}
			j+=pitch-width;
		}
		data.position=0;
		
		surface.bitmap.bitmapData.setPixels( new Rectangle( x,y,width,height ),data );
		
		return 0;
	}
}

//***** gxtkSurface *****

class gxtkSurface{
	internal var bitmap:Bitmap;
	internal var rect:Rectangle;
	internal var scratch:BitmapData;
	
	function gxtkSurface( bitmap:Bitmap ){
		SetBitmap( bitmap );
	}
	
	public function SetBitmap( bitmap:Bitmap ):void{
		this.bitmap=bitmap;
		rect=new Rectangle( 0,0,bitmap.width,bitmap.height );
	}

	//***** GXTK API *****

	public function Discard():int{
		return 0;
	}
	
	public function Width():int{
		return rect.width;
	}

	public function Height():int{
		return rect.height;
	}

	public function Loaded():int{
		return 1;
	}
	
	public function OnUnsafeLoadComplete():Boolean{
		return true;
	}
}

class gxtkInput{

	internal var KEY_LMB:int=1;
	internal var KEY_TOUCH0:int=0x180;

	internal var keyStates:Array=new Array( 512 );
	internal var charQueue:Array=new Array( 32 );
	internal var charPut:int=0;
	internal var charGet:int=0;
	internal var mouseX:Number=0;
	internal var mouseY:Number=0;
	
	function gxtkInput(){
	
		for( var i:int=0;i<512;++i ){
			keyStates[i]=0;
		}

		var stage:Stage=game.stage;
	
		stage.addEventListener( KeyboardEvent.KEY_DOWN,function( e:KeyboardEvent ):void{
			OnKeyDown( e.keyCode );
			if( e.charCode!=0 ){
				PutChar( e.charCode );
			}else{
				var chr:int=KeyToChar( e.keyCode );
				if( chr ) PutChar( chr );
			}
		} );
		
		stage.addEventListener( KeyboardEvent.KEY_UP,function( e:KeyboardEvent ):void{
			OnKeyUp( e.keyCode );
		} );
		
		stage.addEventListener( MouseEvent.MOUSE_DOWN,function( e:MouseEvent ):void{
			OnKeyDown( KEY_LMB );
		} );
		
		stage.addEventListener( MouseEvent.MOUSE_UP,function( e:MouseEvent ):void{
			OnKeyUp( KEY_LMB );
		} );
		
		stage.addEventListener( MouseEvent.MOUSE_MOVE,function( e:MouseEvent ):void{
			OnMouseMove( e.localX,e.localY );
		} );
	}
	
	internal function KeyToChar( key:int ):int{
		switch( key ){
		case 8:case 9:case 13:case 27:
			return key;
		case 33:case 34:case 35:case 36:case 37:case 38:case 39:case 40:case 45:
			return key | 0x10000;
		case 46:
			return 127;
		}
		return 0;
	}
	
	internal function BeginUpdate():void{
	}
	
	internal function EndUpdate():void{
		for( var i:int=0;i<512;++i ){
			keyStates[i]&=0x100;
		}
		charGet=0;
		charPut=0;
	}
	
	internal function OnKeyDown( key:int ):void{
		if( (keyStates[key]&0x100)==0 ){
			keyStates[key]|=0x100;
			++keyStates[key];
		}
	}

	internal function OnKeyUp( key:int ):void{
		keyStates[key]&=0xff;
	}
	
	internal function PutChar( chr:int ):void{
		if( chr==0 ) return;
		if( charPut-charGet<32 ){
			charQueue[charPut & 31]=chr;
			charPut+=1;
		}
	}
	
	internal function OnMouseMove( x:Number,y:Number ):void{
		mouseX=x;
		mouseY=y;
	}

	//***** GXTK API *****
	
	public function SetKeyboardEnabled( enabled:int ):int{
		return 0;
	}
	
	public function KeyDown( key:int ):int{
		if( key>0 && key<512 ){
			if( key==KEY_TOUCH0 ) key=KEY_LMB;
			return keyStates[key] >> 8;
		}
		return 0;
	}

	public function KeyHit( key:int ):int{
		if( key>0 && key<512 ){
			if( key==KEY_TOUCH0 ) key=KEY_LMB;
			return keyStates[key] & 0xff;
		}
		return 0;
	}

	public function GetChar():int{
		if( charGet!=charPut ){
			var chr:int=charQueue[charGet & 31];
			charGet+=1;
			return chr;
		}
		return 0;
	}
	
	public function MouseX():Number{
		return mouseX;
	}

	public function MouseY():Number{
		return mouseY;
	}

	public function JoyX( index:int ):Number{
		return 0;
	}
	
	public function JoyY( index:int ):Number{
		return 0;
	}
	
	public function JoyZ( index:int ):Number{
		return 0;
	}
	
	public function TouchX( index:int ):Number{
		return mouseX;
	}

	public function TouchY( index:int ):Number{
		return mouseY;
	}
	
	public function AccelX():Number{
		return 0;
	}
	
	public function AccelY():Number{
		return 0;
	}
	
	public function AccelZ():Number{
		return 0;
	}
}

class gxtkChannel{
	internal var channel:SoundChannel;	//null then not playing
	internal var sample:gxtkSample;
	internal var loops:int;
	internal var transform:SoundTransform=new SoundTransform();
	internal var pausepos:Number;
	internal var state:int;
}

class gxtkAudio{

	internal var music:gxtkSample;

	internal var channels:Array=new Array( 33 );

	internal var loop_kludge:int=1;

	function gxtkAudio(){
		for( var i:int=0;i<33;++i ){
			channels[i]=new gxtkChannel();
		}
	}
	
	internal function OnSuspend():void{
		for( var i:int=0;i<33;++i ){
			var chan:gxtkChannel=channels[i];
			if( chan.state==1 ){
				chan.pausepos=chan.channel.position;
				chan.channel.stop();
			}
		}
	}
	
	internal function OnResume():void{
		for( var i:int=0;i<33;++i ){
			var chan:gxtkChannel=channels[i];
			if( chan.state==1 ){
				if( loop_kludge ){
					chan.channel=chan.sample.sound.play( chan.pausepos,0,chan.transform );
					if( chan.loops ) chan.channel.addEventListener( Event.SOUND_COMPLETE,SoundComplete );
				}else{
					chan.channel=chan.sample.sound.play( chan.pausepos,chan.loops,chan.transform );
				}
			}
		}
	}
	
	internal function SoundComplete( ev:Event ):void{
		if( !loop_kludge ) return;
		for( var i:int=0;i<33;++i ){
			var chan:gxtkChannel=channels[i];
			if( chan.state==1 && chan.channel==ev.target && chan.loops ){
				chan.channel=chan.sample.sound.play( 0,0,chan.transform );
				chan.channel.addEventListener( Event.SOUND_COMPLETE,SoundComplete );
				break;
			}
		}
	}
	
	//***** GXTK API *****
	
	public function LoadSample( path:String ):gxtkSample{
		var sound:Sound=game.loadSound( path );
		if( sound ) return new gxtkSample( sound );
		return null;
	}
	
	public function PlaySample( sample:gxtkSample,channel:int,flags:int ):int{
		var chan:gxtkChannel=channels[channel];
		
		if( chan.state!=0 ) chan.channel.stop();

		chan.sample=sample;
		chan.loops=flags ? 0x7fffffff : 0;
		chan.state=1;
		if( loop_kludge ){
			chan.channel=sample.sound.play( 0,0,chan.transform );
			chan.channel.addEventListener( Event.SOUND_COMPLETE,SoundComplete );
		}else{
			chan.channel=sample.sound.play( 0,chan.loops,chan.transform );
		}

		return 0;
	}
	
	public function StopChannel( channel:int ):int{
		var chan:gxtkChannel=channels[channel];
		
		if( chan.state!=0 ){
			chan.channel.stop();
			chan.channel=null;
			chan.sample=null;
			chan.state=0;
		}
		return 0;
	}
	
	public function PauseChannel( channel:int ):int{
		var chan:gxtkChannel=channels[channel];
		
		if( chan.state==1 ){
			chan.pausepos=chan.channel.position;
			chan.channel.stop();
			chan.state=2;
		}
		return 0;
	}
	
	public function ResumeChannel( channel:int ):int{
		var chan:gxtkChannel=channels[channel];
		
		if( chan.state==2 ){
			chan.channel=chan.sample.sound.play( chan.pausepos,chan.loops,chan.transform );
			chan.state=1;
		}
		return 0;
	}
	
	public function ChannelState( channel:int ):int{
		return -1;
	}
	
	public function SetVolume( channel:int,volume:Number ):int{
		var chan:gxtkChannel=channels[channel];
		
		chan.transform.volume=volume;

		if( chan.state!=0 ) chan.channel.soundTransform=chan.transform;

		return 0;
	}
	
	public function SetPan( channel:int,pan:Number ):int{
		var chan:gxtkChannel=channels[channel];
		
		chan.transform.pan=pan;

		if( chan.state!=0 ) chan.channel.soundTransform=chan.transform;

		return 0;
	}
	
	public function SetRate( channel:int,rate:Number ):int{
		return -1;
	}
	
	public function PlayMusic( path:String,flags:int ):int{
		StopMusic();
		
		music=LoadSample( path );
		if( !music ) return -1;
		
		PlaySample( music,32,flags );
		return 0;
	}
	
	public function StopMusic():int{
		StopChannel( 32 );
		
		if( music ){
			music.Discard();
			music=null;
		}
		return 0;
	}
	
	public function PauseMusic():int{
		PauseChannel( 32 );
		
		return 0;
	}
	
	public function ResumeMusic():int{
		ResumeChannel( 32 );
		
		return 0;
	}
	
	public function MusicState():int{
		return ChannelState( 32 );
	}
	
	public function SetMusicVolume( volume:Number ):int{
		SetVolume( 32,volume );
		return 0;
	}
}

class gxtkSample{

	internal var sound:Sound;

	function gxtkSample( sound:Sound ){
		this.sound=sound;
	}
	
	public function Discard():int{
		return 0;
	}
	
}

class BBThread{

	internal var running:Boolean=false;
	
	public function Start():void{
		Run__UNSAFE__();
	}
	
	public function IsRunning():Boolean{
		return running;
	}
	
	public function Run__UNSAFE__():void{
	}
}

class BBAsyncImageLoaderThread extends BBThread{

	internal var _device:gxtkGraphics;
	internal var _path:String;
	internal var _surface:gxtkSurface;

	override public function Start():void{
		
		var thread:BBAsyncImageLoaderThread=this;
		
		var loader:Loader=new Loader();
		
		loader.contentLoaderInfo.addEventListener( Event.COMPLETE,onLoaded );
		loader.contentLoaderInfo.addEventListener( IOErrorEvent.IO_ERROR,onIoError );
		loader.contentLoaderInfo.addEventListener( SecurityErrorEvent.SECURITY_ERROR,onSecurityError );
		
		function onLoaded( e:Event ):void{
			thread._surface=new gxtkSurface( e.target.content );
			thread.running=false;
		}
		
		function onIoError( e:IOErrorEvent ):void{
			thread._surface=null;
			thread.running=false;
		}

		function onSecurityError( e:SecurityErrorEvent ):void{
			thread._surface=null;
			thread.running=false;
		}
		
		thread.running=true;
		
		loader.load( game.urlRequest( thread._path ) );
	}

}

class BBAsyncSoundLoaderThread extends BBThread{

	internal var _device:gxtkAudio;
	internal var _path:String;
	internal var _sample:gxtkSample;

	override public function Start():void{
		
		var thread:BBAsyncSoundLoaderThread=this;
		
		var sound:Sound=new Sound();
		
		sound.addEventListener( Event.COMPLETE,onLoaded );
		sound.addEventListener( IOErrorEvent.IO_ERROR,onIoError );
		sound.addEventListener( SecurityErrorEvent.SECURITY_ERROR,onSecurityError );
		
		function onLoaded( e:Event ):void{
			thread._sample=new gxtkSample( sound );
			thread.running=false;
		}
		
		function onIoError( e:IOErrorEvent ):void{
			thread._sample=null;
			thread.running=false;
		}

		function onSecurityError( e:SecurityErrorEvent ):void{
			thread._sample=null;
			thread.running=false;
		}
		
		thread.running=true;
		
		sound.load( game.urlRequest( thread._path ) );
	}
}
class bb_app_App extends Object{
	public function g_App_new():bb_app_App{
		bb_app_device=(new bb_app_AppDevice).g_AppDevice_new(this);
		return this;
	}
	public function m_OnCreate():int{
		return 0;
	}
	public function m_OnUpdate():int{
		return 0;
	}
	public function m_OnSuspend():int{
		return 0;
	}
	public function m_OnResume():int{
		return 0;
	}
	public function m_OnRender():int{
		return 0;
	}
	public function m_OnLoading():int{
		return 0;
	}
}
class bb_ld_LDApp extends bb_app_App{
	public function g_LDApp_new():bb_ld_LDApp{
		super.g_App_new();
		return this;
	}
	internal static var g_ScreenHeight:int;
	internal static var g_ScreenWidth:int;
	internal static var g_RefreshRate:int;
	internal static var g_Delta:Number;
	public override function m_OnCreate():int{
		bb_gfx_GFX.g_Init();
		bb_screenmanager_ScreenManager.g_Init();
		bb_sfx_SFX.g_Init();
		bb_controls_Controls.g_Init();
		bb_raztext_RazText.g_SetTextSheet(bb_graphics_LoadImage("gfx/fonts.png",1,bb_graphics_Image.g_DefaultFlags));
		bb_screenmanager_ScreenManager.g_AddScreen("game",((new bb_gamescreen_GameScreen).g_GameScreen_new()));
		bb_screenmanager_ScreenManager.g_AddScreen("title",((new bb_titlescreen_TitleScreen).g_TitleScreen_new()));
		bb_sfx_SFX.g_AddMusic("ambient","ambient.mp3");
		bb_sfx_SFX.g_AddMusic("chase","chase.mp3");
		bb_screenmanager_ScreenManager.g_SetFadeColour(0.0,0.0,0.0);
		bb_screenmanager_ScreenManager.g_SetFadeRate(0.1);
		bb_screenmanager_ScreenManager.g_SetScreen("title");
		if(bb_controls_Controls.g_ControlMethod==2){
			g_RefreshRate=30;
		}else{
			g_RefreshRate=60;
		}
		bb_app_SetUpdateRate(g_RefreshRate);
		g_Delta=(g_RefreshRate)/60.0;
		bb_autofit_SetVirtualDisplay(g_ScreenWidth,g_ScreenHeight,1.0);
		return 0;
	}
	internal static var g_TargetScreenX:Number;
	internal static var g_ActualScreenX:int;
	internal static var g_ScreenMoveRate:Number;
	internal static var g_TargetScreenY:Number;
	internal static var g_ActualScreenY:int;
	internal static var g_ScreenX:int;
	internal static var g_ScreenY:int;
	public override function m_OnUpdate():int{
		bb_controls_Controls.g_Update();
		g_ActualScreenX=(((g_ActualScreenX)+(g_TargetScreenX-(g_ActualScreenX))*g_ScreenMoveRate)|0);
		g_ActualScreenY=(((g_ActualScreenY)+(g_TargetScreenY-(g_ActualScreenY))*g_ScreenMoveRate)|0);
		g_ScreenX=g_ActualScreenX;
		g_ScreenY=g_ActualScreenY;
		bb_screenmanager_ScreenManager.g_Update();
		return 0;
	}
	public override function m_OnRender():int{
		bb_autofit_UpdateVirtualDisplay(true,true);
		bb_graphics_Cls(0.0,0.0,0.0);
		bb_screenmanager_ScreenManager.g_Render();
		return 0;
	}
	internal static var g_level:bb_level_Level;
	public static function g_SetScreenTarget(t_tX:Number,t_tY:Number):void{
		g_TargetScreenX=t_tX-(g_ScreenWidth)*0.5;
		g_TargetScreenY=t_tY-(g_ScreenHeight)*0.5;
	}
	public static function g_SetScreenPosition(t_tX:Number,t_tY:Number):void{
		g_SetScreenTarget(t_tX,t_tY);
		g_ActualScreenX=((g_TargetScreenX)|0);
		g_ActualScreenY=((g_TargetScreenY)|0);
	}
}
class bb_app_AppDevice extends gxtkApp{
	internal var f_app:bb_app_App=null;
	public function g_AppDevice_new(t_app:bb_app_App):bb_app_AppDevice{
		this.f_app=t_app;
		bb_graphics_SetGraphicsDevice(this.GraphicsDevice());
		bb_input_SetInputDevice(this.InputDevice());
		bb_audio_SetAudioDevice(this.AudioDevice());
		return this;
	}
	public function g_AppDevice_new2():bb_app_AppDevice{
		return this;
	}
	public override function OnCreate():int{
		bb_graphics_SetFont(null,32);
		return this.f_app.m_OnCreate();
	}
	public override function OnUpdate():int{
		return this.f_app.m_OnUpdate();
	}
	public override function OnSuspend():int{
		return this.f_app.m_OnSuspend();
	}
	public override function OnResume():int{
		return this.f_app.m_OnResume();
	}
	public override function OnRender():int{
		bb_graphics_BeginRender();
		var t_r:int=this.f_app.m_OnRender();
		bb_graphics_EndRender();
		return t_r;
	}
	public override function OnLoading():int{
		bb_graphics_BeginRender();
		var t_r:int=this.f_app.m_OnLoading();
		bb_graphics_EndRender();
		return t_r;
	}
	internal var f_updateRate:int=0;
	public override function SetUpdateRate(t_hertz:int):int{
		super.SetUpdateRate(t_hertz);
		this.f_updateRate=t_hertz;
		return 0;
	}
}
var bb_graphics_device:gxtkGraphics;
internal function bb_graphics_SetGraphicsDevice(t_dev:gxtkGraphics):int{
	bb_graphics_device=t_dev;
	return 0;
}
var bb_input_device:gxtkInput;
internal function bb_input_SetInputDevice(t_dev:gxtkInput):int{
	bb_input_device=t_dev;
	return 0;
}
var bb_audio_device:gxtkAudio;
internal function bb_audio_SetAudioDevice(t_dev:gxtkAudio):int{
	bb_audio_device=t_dev;
	return 0;
}
var bb_app_device:bb_app_AppDevice;
internal function bbMain():int{
	(new bb_ld_LDApp).g_LDApp_new();
	return 0;
}
class bb_graphics_Image extends Object{
	internal static var g_DefaultFlags:int;
	public function g_Image_new():bb_graphics_Image{
		return this;
	}
	internal var f_surface:gxtkSurface=null;
	internal var f_width:int=0;
	internal var f_height:int=0;
	internal var f_frames:Array=[];
	internal var f_flags:int=0;
	internal var f_tx:Number=.0;
	internal var f_ty:Number=.0;
	public function m_SetHandle(t_tx:Number,t_ty:Number):int{
		this.f_tx=t_tx;
		this.f_ty=t_ty;
		this.f_flags=this.f_flags&-2;
		return 0;
	}
	public function m_ApplyFlags(t_iflags:int):int{
		this.f_flags=t_iflags;
		if((this.f_flags&2)!=0){
			var t_:Array=this.f_frames;
			var t_2:int=0;
			while(t_2<t_.length){
				var t_f:bb_graphics_Frame=t_[t_2];
				t_2=t_2+1;
				t_f.f_x+=1;
			}
			this.f_width-=2;
		}
		if((this.f_flags&4)!=0){
			var t_3:Array=this.f_frames;
			var t_4:int=0;
			while(t_4<t_3.length){
				var t_f2:bb_graphics_Frame=t_3[t_4];
				t_4=t_4+1;
				t_f2.f_y+=1;
			}
			this.f_height-=2;
		}
		if((this.f_flags&1)!=0){
			this.m_SetHandle((this.f_width)/2.0,(this.f_height)/2.0);
		}
		if(this.f_frames.length==1 && this.f_frames[0].f_x==0 && this.f_frames[0].f_y==0 && this.f_width==this.f_surface.Width() && this.f_height==this.f_surface.Height()){
			this.f_flags|=65536;
		}
		return 0;
	}
	public function m_Init(t_surf:gxtkSurface,t_nframes:int,t_iflags:int):bb_graphics_Image{
		this.f_surface=t_surf;
		this.f_width=((this.f_surface.Width()/t_nframes)|0);
		this.f_height=this.f_surface.Height();
		this.f_frames=new_object_array(t_nframes);
		for(var t_i:int=0;t_i<t_nframes;t_i=t_i+1){
			this.f_frames[t_i]=(new bb_graphics_Frame).g_Frame_new(t_i*this.f_width,0);
		}
		this.m_ApplyFlags(t_iflags);
		return this;
	}
	internal var f_source:bb_graphics_Image=null;
	public function m_Grab(t_x:int,t_y:int,t_iwidth:int,t_iheight:int,t_nframes:int,t_iflags:int,t_source:bb_graphics_Image):bb_graphics_Image{
		this.f_source=t_source;
		this.f_surface=t_source.f_surface;
		this.f_width=t_iwidth;
		this.f_height=t_iheight;
		this.f_frames=new_object_array(t_nframes);
		var t_ix:int=t_x;
		var t_iy:int=t_y;
		for(var t_i:int=0;t_i<t_nframes;t_i=t_i+1){
			if(t_ix+this.f_width>t_source.f_width){
				t_ix=0;
				t_iy+=this.f_height;
			}
			if(t_ix+this.f_width>t_source.f_width || t_iy+this.f_height>t_source.f_height){
				error("Image frame outside surface");
			}
			this.f_frames[t_i]=(new bb_graphics_Frame).g_Frame_new(t_ix+t_source.f_frames[0].f_x,t_iy+t_source.f_frames[0].f_y);
			t_ix+=this.f_width;
		}
		this.m_ApplyFlags(t_iflags);
		return this;
	}
	public function m_GrabImage(t_x:int,t_y:int,t_width:int,t_height:int,t_frames:int,t_flags:int):bb_graphics_Image{
		if(this.f_frames.length!=1){
			return null;
		}
		return ((new bb_graphics_Image).g_Image_new()).m_Grab(t_x,t_y,t_width,t_height,t_frames,t_flags,this);
	}
	public function m_Width():int{
		return this.f_width;
	}
	public function m_Height():int{
		return this.f_height;
	}
	public function m_Frames():int{
		return this.f_frames.length;
	}
}
class bb_graphics_GraphicsContext extends Object{
	public function g_GraphicsContext_new():bb_graphics_GraphicsContext{
		return this;
	}
	internal var f_defaultFont:bb_graphics_Image=null;
	internal var f_font:bb_graphics_Image=null;
	internal var f_firstChar:int=0;
	internal var f_matrixSp:int=0;
	internal var f_ix:Number=1.0;
	internal var f_iy:Number=.0;
	internal var f_jx:Number=.0;
	internal var f_jy:Number=1.0;
	internal var f_tx:Number=.0;
	internal var f_ty:Number=.0;
	internal var f_tformed:int=0;
	internal var f_matDirty:int=0;
	internal var f_color_r:Number=.0;
	internal var f_color_g:Number=.0;
	internal var f_color_b:Number=.0;
	internal var f_alpha:Number=.0;
	internal var f_blend:int=0;
	internal var f_scissor_x:Number=.0;
	internal var f_scissor_y:Number=.0;
	internal var f_scissor_width:Number=.0;
	internal var f_scissor_height:Number=.0;
	public function m_Validate():int{
		if((this.f_matDirty)!=0){
			bb_graphics_renderDevice.SetMatrix(bb_graphics_context.f_ix,bb_graphics_context.f_iy,bb_graphics_context.f_jx,bb_graphics_context.f_jy,bb_graphics_context.f_tx,bb_graphics_context.f_ty);
			this.f_matDirty=0;
		}
		return 0;
	}
	internal var f_matrixStack:Array=new_number_array(192);
}
var bb_graphics_context:bb_graphics_GraphicsContext;
internal function bb_data_FixDataPath(t_path:String):String{
	var t_i:int=t_path.indexOf(":/",0);
	if(t_i!=-1 && t_path.indexOf("/",0)==t_i+1){
		return t_path;
	}
	if(string_startswith(t_path,"./") || string_startswith(t_path,"/")){
		return t_path;
	}
	return "monkey://data/"+t_path;
}
class bb_graphics_Frame extends Object{
	internal var f_x:int=0;
	internal var f_y:int=0;
	public function g_Frame_new(t_x:int,t_y:int):bb_graphics_Frame{
		this.f_x=t_x;
		this.f_y=t_y;
		return this;
	}
	public function g_Frame_new2():bb_graphics_Frame{
		return this;
	}
}
internal function bb_graphics_LoadImage(t_path:String,t_frameCount:int,t_flags:int):bb_graphics_Image{
	var t_surf:gxtkSurface=bb_graphics_device.LoadSurface(bb_data_FixDataPath(t_path));
	if((t_surf)!=null){
		return ((new bb_graphics_Image).g_Image_new()).m_Init(t_surf,t_frameCount,t_flags);
	}
	return null;
}
internal function bb_graphics_LoadImage2(t_path:String,t_frameWidth:int,t_frameHeight:int,t_frameCount:int,t_flags:int):bb_graphics_Image{
	var t_atlas:bb_graphics_Image=bb_graphics_LoadImage(t_path,1,0);
	if((t_atlas)!=null){
		return t_atlas.m_GrabImage(0,0,t_frameWidth,t_frameHeight,t_frameCount,t_flags);
	}
	return null;
}
internal function bb_graphics_SetFont(t_font:bb_graphics_Image,t_firstChar:int):int{
	if(!((t_font)!=null)){
		if(!((bb_graphics_context.f_defaultFont)!=null)){
			bb_graphics_context.f_defaultFont=bb_graphics_LoadImage("mojo_font.png",96,2);
		}
		t_font=bb_graphics_context.f_defaultFont;
		t_firstChar=32;
	}
	bb_graphics_context.f_font=t_font;
	bb_graphics_context.f_firstChar=t_firstChar;
	return 0;
}
var bb_graphics_renderDevice:gxtkGraphics;
internal function bb_graphics_SetMatrix(t_ix:Number,t_iy:Number,t_jx:Number,t_jy:Number,t_tx:Number,t_ty:Number):int{
	bb_graphics_context.f_ix=t_ix;
	bb_graphics_context.f_iy=t_iy;
	bb_graphics_context.f_jx=t_jx;
	bb_graphics_context.f_jy=t_jy;
	bb_graphics_context.f_tx=t_tx;
	bb_graphics_context.f_ty=t_ty;
	bb_graphics_context.f_tformed=((t_ix!=1.0 || t_iy!=0.0 || t_jx!=0.0 || t_jy!=1.0 || t_tx!=0.0 || t_ty!=0.0)?1:0);
	bb_graphics_context.f_matDirty=1;
	return 0;
}
internal function bb_graphics_SetMatrix2(t_m:Array):int{
	bb_graphics_SetMatrix(t_m[0],t_m[1],t_m[2],t_m[3],t_m[4],t_m[5]);
	return 0;
}
internal function bb_graphics_SetColor(t_r:Number,t_g:Number,t_b:Number):int{
	bb_graphics_context.f_color_r=t_r;
	bb_graphics_context.f_color_g=t_g;
	bb_graphics_context.f_color_b=t_b;
	bb_graphics_renderDevice.SetColor(t_r,t_g,t_b);
	return 0;
}
internal function bb_graphics_SetAlpha(t_alpha:Number):int{
	bb_graphics_context.f_alpha=t_alpha;
	bb_graphics_renderDevice.SetAlpha(t_alpha);
	return 0;
}
internal function bb_graphics_SetBlend(t_blend:int):int{
	bb_graphics_context.f_blend=t_blend;
	bb_graphics_renderDevice.SetBlend(t_blend);
	return 0;
}
internal function bb_graphics_DeviceWidth():int{
	return bb_graphics_device.Width();
}
internal function bb_graphics_DeviceHeight():int{
	return bb_graphics_device.Height();
}
internal function bb_graphics_SetScissor(t_x:Number,t_y:Number,t_width:Number,t_height:Number):int{
	bb_graphics_context.f_scissor_x=t_x;
	bb_graphics_context.f_scissor_y=t_y;
	bb_graphics_context.f_scissor_width=t_width;
	bb_graphics_context.f_scissor_height=t_height;
	bb_graphics_renderDevice.SetScissor(((t_x)|0),((t_y)|0),((t_width)|0),((t_height)|0));
	return 0;
}
internal function bb_graphics_BeginRender():int{
	if(!((bb_graphics_device.Mode())!=0)){
		return 0;
	}
	bb_graphics_renderDevice=bb_graphics_device;
	bb_graphics_context.f_matrixSp=0;
	bb_graphics_SetMatrix(1.0,0.0,0.0,1.0,0.0,0.0);
	bb_graphics_SetColor(255.0,255.0,255.0);
	bb_graphics_SetAlpha(1.0);
	bb_graphics_SetBlend(0);
	bb_graphics_SetScissor(0.0,0.0,(bb_graphics_DeviceWidth()),(bb_graphics_DeviceHeight()));
	return 0;
}
internal function bb_graphics_EndRender():int{
	bb_graphics_renderDevice=null;
	return 0;
}
class bb_gfx_GFX extends Object{
	internal static var g_Tileset:bb_graphics_Image;
	internal static var g_Overlay:bb_graphics_Image;
	internal static var g_Title:bb_graphics_Image;
	public static function g_Init():void{
		g_Tileset=bb_graphics_LoadImage("gfx/sheet.png",1,bb_graphics_Image.g_DefaultFlags);
		g_Overlay=bb_graphics_LoadImage("gfx/overlay.png",1,bb_graphics_Image.g_DefaultFlags);
		g_Title=g_Tileset.m_GrabImage(345,448,167,64,1,1);
	}
	public static function g_Draw(t_tImage:bb_graphics_Image,t_tX:Number,t_tY:Number,t_tF:int,t_Follow:Boolean):void{
		if(t_Follow==true){
			bb_graphics_DrawImage(t_tImage,t_tX-(bb_ld_LDApp.g_ScreenX),t_tY-(bb_ld_LDApp.g_ScreenY),t_tF);
		}else{
			bb_graphics_DrawImage(t_tImage,t_tX,t_tY,t_tF);
		}
	}
}
class bb_screenmanager_ScreenManager extends Object{
	internal static var g_Screens:bb_map_StringMap;
	internal static var g_FadeAlpha:Number;
	internal static var g_FadeRate:Number;
	internal static var g_FadeRed:Number;
	internal static var g_FadeGreen:Number;
	internal static var g_FadeBlue:Number;
	internal static var g_FadeMode:int;
	public static function g_Init():void{
		g_Screens=(new bb_map_StringMap).g_StringMap_new();
		g_FadeAlpha=0.0;
		g_FadeRate=0.01;
		g_FadeRed=0.0;
		g_FadeGreen=0.0;
		g_FadeBlue=0.0;
		g_FadeMode=0;
	}
	internal static var g_gameScreen:bb_gamescreen_GameScreen;
	public static function g_AddScreen(t_tName:String,t_tScreen:bb_screen_Screen):void{
		g_Screens.m_Set(t_tName,t_tScreen);
		if(t_tName=="game"){
			g_gameScreen=((t_tScreen) as bb_gamescreen_GameScreen);
		}
	}
	public static function g_SetFadeColour(t_tR:Number,t_tG:Number,t_tB:Number):void{
		g_FadeRed=bb_math_Clamp2(t_tR,0.0,255.0);
		g_FadeGreen=bb_math_Clamp2(t_tG,0.0,255.0);
		g_FadeBlue=bb_math_Clamp2(t_tB,0.0,255.0);
	}
	public static function g_SetFadeRate(t_tRate:Number):void{
		g_FadeRate=bb_math_Clamp2(t_tRate,0.001,1.0);
	}
	internal static var g_ActiveScreen:bb_screen_Screen;
	internal static var g_ActiveScreenName:String;
	public static function g_SetScreen(t_tName:String):void{
		if(g_ActiveScreen!=null){
			g_ActiveScreen.m_OnScreenEnd();
		}
		g_ActiveScreen=g_Screens.m_Get(t_tName);
		g_ActiveScreen.m_OnScreenStart();
		g_FadeMode=1;
		g_ActiveScreenName=t_tName;
	}
	internal static var g_NextScreenName:String;
	public static function g_Update():void{
		if(g_ActiveScreen!=null){
			g_ActiveScreen.m_Update();
		}
		var t_:int=g_FadeMode;
		if(t_==0){
			g_FadeAlpha-=g_FadeRate;
			if(g_FadeAlpha<=0.0){
				g_FadeAlpha=0.0;
				g_FadeMode=1;
			}
		}else{
			if(t_==1){
			}else{
				if(t_==2){
					g_FadeAlpha+=g_FadeRate;
					if(g_FadeAlpha>=1.0){
						g_FadeAlpha=1.0;
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
	public static function g_Render():void{
		bb_graphics_SetColor(255.0,255.0,255.0);
		bb_graphics_SetAlpha(1.0);
		if(g_ActiveScreen!=null){
			g_ActiveScreen.m_Render();
		}
		bb_graphics_SetColor(255.0,255.0,255.0);
		bb_graphics_SetAlpha(1.0);
		if(bb_controls_Controls.g_ControlMethod==2){
			bb_controls_Controls.g_TCMove.m_DoRenderRing();
			bb_controls_Controls.g_TCMove.m_DoRenderStick();
			bb_controls_Controls.g_TCAction1.m_Render();
			bb_controls_Controls.g_TCAction2.m_Render();
			bb_controls_Controls.g_TCEscapeKey.m_Render();
		}
		if(g_FadeMode!=1){
			bb_graphics_SetColor(g_FadeRed,g_FadeGreen,g_FadeBlue);
			bb_graphics_SetAlpha(g_FadeAlpha);
			bb_graphics_DrawRect(0.0,0.0,(bb_graphics_DeviceWidth()),(bb_graphics_DeviceHeight()));
		}
	}
	public static function g_ChangeScreen(t_tName:String):void{
		if(t_tName!=g_ActiveScreenName){
			if(g_Screens.m_Contains(t_tName)){
				g_NextScreenName=t_tName;
				g_FadeMode=2;
			}
		}
	}
}
class bb_screen_Screen extends Object{
	public function g_Screen_new():bb_screen_Screen{
		return this;
	}
	public function m_OnScreenEnd():void{
	}
	public function m_OnScreenStart():void{
	}
	public function m_Update():void{
	}
	public function m_Render():void{
	}
}
class bb_map_Map extends Object{
	public function g_Map_new():bb_map_Map{
		return this;
	}
	internal var f_root:bb_map_Node=null;
	public function m_Compare(t_lhs:String,t_rhs:String):int{
		return 0;
	}
	public function m_RotateLeft(t_node:bb_map_Node):int{
		var t_child:bb_map_Node=t_node.f_right;
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
			this.f_root=t_child;
		}
		t_child.f_left=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public function m_RotateRight(t_node:bb_map_Node):int{
		var t_child:bb_map_Node=t_node.f_left;
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
			this.f_root=t_child;
		}
		t_child.f_right=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public function m_InsertFixup(t_node:bb_map_Node):int{
		while(((t_node.f_parent)!=null) && t_node.f_parent.f_color==-1 && ((t_node.f_parent.f_parent)!=null)){
			if(t_node.f_parent==t_node.f_parent.f_parent.f_left){
				var t_uncle:bb_map_Node=t_node.f_parent.f_parent.f_right;
				if(((t_uncle)!=null) && t_uncle.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle.f_color=1;
					t_uncle.f_parent.f_color=-1;
					t_node=t_uncle.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_right){
						t_node=t_node.f_parent;
						this.m_RotateLeft(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					this.m_RotateRight(t_node.f_parent.f_parent);
				}
			}else{
				var t_uncle2:bb_map_Node=t_node.f_parent.f_parent.f_left;
				if(((t_uncle2)!=null) && t_uncle2.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle2.f_color=1;
					t_uncle2.f_parent.f_color=-1;
					t_node=t_uncle2.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_left){
						t_node=t_node.f_parent;
						this.m_RotateRight(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					this.m_RotateLeft(t_node.f_parent.f_parent);
				}
			}
		}
		this.f_root.f_color=1;
		return 0;
	}
	public function m_Set(t_key:String,t_value:bb_screen_Screen):Boolean{
		var t_node:bb_map_Node=this.f_root;
		var t_parent:bb_map_Node=null;
		var t_cmp:int=0;
		while((t_node)!=null){
			t_parent=t_node;
			t_cmp=this.m_Compare(t_key,t_node.f_key);
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
		t_node=(new bb_map_Node).g_Node_new(t_key,t_value,-1,t_parent);
		if((t_parent)!=null){
			if(t_cmp>0){
				t_parent.f_right=t_node;
			}else{
				t_parent.f_left=t_node;
			}
			this.m_InsertFixup(t_node);
		}else{
			this.f_root=t_node;
		}
		return true;
	}
	public function m_FindNode(t_key:String):bb_map_Node{
		var t_node:bb_map_Node=this.f_root;
		while((t_node)!=null){
			var t_cmp:int=this.m_Compare(t_key,t_node.f_key);
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
	public function m_Get(t_key:String):bb_screen_Screen{
		var t_node:bb_map_Node=this.m_FindNode(t_key);
		if((t_node)!=null){
			return t_node.f_value;
		}
		return null;
	}
	public function m_Contains(t_key:String):Boolean{
		return this.m_FindNode(t_key)!=null;
	}
}
class bb_map_StringMap extends bb_map_Map{
	public function g_StringMap_new():bb_map_StringMap{
		super.g_Map_new();
		return this;
	}
	public override function m_Compare(t_lhs:String,t_rhs:String):int{
		return string_compare(t_lhs,t_rhs);
	}
}
class bb_sfx_SFX extends Object{
	internal static var g_ActiveChannel:int;
	internal static var g_Sounds:bb_map_StringMap2;
	internal static var g_Musics:bb_map_StringMap3;
	internal static var g_SoundFileAppendix:String;
	public static function g_Init():void{
		g_ActiveChannel=0;
		g_Sounds=(new bb_map_StringMap2).g_StringMap_new();
		g_Musics=(new bb_map_StringMap3).g_StringMap_new();
		g_SoundFileAppendix=".mp3";
	}
	public static function g_AddMusic(t_tName:String,t_tFile:String):void{
		g_Musics.m_Set2(t_tName,t_tFile);
	}
	internal static var g_MusicActive:Boolean;
	internal static var g_CurrentMusic:String;
	public static function g_Music(t_tMus:String,t_tLoop:int):void{
		if(g_MusicActive==false){
			return;
		}
		if(!g_Musics.m_Contains(t_tMus)){
			error("Music "+t_tMus+" does not appear to exist");
		}
		if(t_tMus!=g_CurrentMusic || (bb_audio_MusicState()==-1 || bb_audio_MusicState()==0)){
			bb_audio_PlayMusic("mus/"+g_Musics.m_Get(t_tMus),t_tLoop);
			g_CurrentMusic=t_tMus;
		}
	}
}
class bb_audio_Sound extends Object{
}
class bb_map_Map2 extends Object{
	public function g_Map_new():bb_map_Map2{
		return this;
	}
}
class bb_map_StringMap2 extends bb_map_Map2{
	public function g_StringMap_new():bb_map_StringMap2{
		super.g_Map_new();
		return this;
	}
}
class bb_map_Map3 extends Object{
	public function g_Map_new():bb_map_Map3{
		return this;
	}
	internal var f_root:bb_map_Node2=null;
	public function m_Compare(t_lhs:String,t_rhs:String):int{
		return 0;
	}
	public function m_RotateLeft2(t_node:bb_map_Node2):int{
		var t_child:bb_map_Node2=t_node.f_right;
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
			this.f_root=t_child;
		}
		t_child.f_left=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public function m_RotateRight2(t_node:bb_map_Node2):int{
		var t_child:bb_map_Node2=t_node.f_left;
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
			this.f_root=t_child;
		}
		t_child.f_right=t_node;
		t_node.f_parent=t_child;
		return 0;
	}
	public function m_InsertFixup2(t_node:bb_map_Node2):int{
		while(((t_node.f_parent)!=null) && t_node.f_parent.f_color==-1 && ((t_node.f_parent.f_parent)!=null)){
			if(t_node.f_parent==t_node.f_parent.f_parent.f_left){
				var t_uncle:bb_map_Node2=t_node.f_parent.f_parent.f_right;
				if(((t_uncle)!=null) && t_uncle.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle.f_color=1;
					t_uncle.f_parent.f_color=-1;
					t_node=t_uncle.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_right){
						t_node=t_node.f_parent;
						this.m_RotateLeft2(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					this.m_RotateRight2(t_node.f_parent.f_parent);
				}
			}else{
				var t_uncle2:bb_map_Node2=t_node.f_parent.f_parent.f_left;
				if(((t_uncle2)!=null) && t_uncle2.f_color==-1){
					t_node.f_parent.f_color=1;
					t_uncle2.f_color=1;
					t_uncle2.f_parent.f_color=-1;
					t_node=t_uncle2.f_parent;
				}else{
					if(t_node==t_node.f_parent.f_left){
						t_node=t_node.f_parent;
						this.m_RotateRight2(t_node);
					}
					t_node.f_parent.f_color=1;
					t_node.f_parent.f_parent.f_color=-1;
					this.m_RotateLeft2(t_node.f_parent.f_parent);
				}
			}
		}
		this.f_root.f_color=1;
		return 0;
	}
	public function m_Set2(t_key:String,t_value:String):Boolean{
		var t_node:bb_map_Node2=this.f_root;
		var t_parent:bb_map_Node2=null;
		var t_cmp:int=0;
		while((t_node)!=null){
			t_parent=t_node;
			t_cmp=this.m_Compare(t_key,t_node.f_key);
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
		t_node=(new bb_map_Node2).g_Node_new(t_key,t_value,-1,t_parent);
		if((t_parent)!=null){
			if(t_cmp>0){
				t_parent.f_right=t_node;
			}else{
				t_parent.f_left=t_node;
			}
			this.m_InsertFixup2(t_node);
		}else{
			this.f_root=t_node;
		}
		return true;
	}
	public function m_FindNode(t_key:String):bb_map_Node2{
		var t_node:bb_map_Node2=this.f_root;
		while((t_node)!=null){
			var t_cmp:int=this.m_Compare(t_key,t_node.f_key);
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
	public function m_Contains(t_key:String):Boolean{
		return this.m_FindNode(t_key)!=null;
	}
	public function m_Get(t_key:String):String{
		var t_node:bb_map_Node2=this.m_FindNode(t_key);
		if((t_node)!=null){
			return t_node.f_value;
		}
		return "";
	}
}
class bb_map_StringMap3 extends bb_map_Map3{
	public function g_StringMap_new():bb_map_StringMap3{
		super.g_Map_new();
		return this;
	}
	public override function m_Compare(t_lhs:String,t_rhs:String):int{
		return string_compare(t_lhs,t_rhs);
	}
}
class bb_controls_Controls extends Object{
	internal static var g_TCMove:bb_virtualstick_MyStick;
	internal static var g_TCAction1:bb_touchbutton_TouchButton;
	internal static var g_TCAction2:bb_touchbutton_TouchButton;
	internal static var g_TCEscapeKey:bb_touchbutton_TouchButton;
	internal static var g_TCButtons:Array;
	public static function g_Init():void{
		g_TCMove=(new bb_virtualstick_MyStick).g_MyStick_new();
		g_TCMove.m_SetRing(50.0,(bb_ld_LDApp.g_ScreenHeight-50),40.0);
		g_TCMove.m_SetStick(0.0,0.0,15.0);
		g_TCMove.m_SetDeadZone(0.2);
		g_TCMove.m_SetTriggerDistance(5.0);
		g_TCAction1=(new bb_touchbutton_TouchButton).g_TouchButton_new(bb_ld_LDApp.g_ScreenWidth-60,bb_ld_LDApp.g_ScreenHeight-40,20,20);
		g_TCAction2=(new bb_touchbutton_TouchButton).g_TouchButton_new(bb_ld_LDApp.g_ScreenWidth-30,bb_ld_LDApp.g_ScreenHeight-40,20,20);
		g_TCEscapeKey=(new bb_touchbutton_TouchButton).g_TouchButton_new(bb_ld_LDApp.g_ScreenWidth-20,0,20,20);
		g_TCButtons=new_object_array(3);
		g_TCButtons[0]=g_TCAction1;
		g_TCButtons[1]=g_TCAction2;
		g_TCButtons[2]=g_TCEscapeKey;
	}
	internal static var g_ControlMethod:int;
	internal static var g_LeftHit:Boolean;
	internal static var g_RightHit:Boolean;
	internal static var g_DownHit:Boolean;
	internal static var g_UpHit:Boolean;
	internal static var g_ActionHit:Boolean;
	internal static var g_Action2Hit:Boolean;
	internal static var g_EscapeHit:Boolean;
	internal static var g_LeftKey:int;
	internal static var g_LeftDown:Boolean;
	internal static var g_RightKey:int;
	internal static var g_RightDown:Boolean;
	internal static var g_UpKey:int;
	internal static var g_UpDown:Boolean;
	internal static var g_DownKey:int;
	internal static var g_DownDown:Boolean;
	internal static var g_ActionKey:int;
	internal static var g_ActionDown:Boolean;
	internal static var g_Action2Key:int;
	internal static var g_Action2Down:Boolean;
	internal static var g_EscapeKey:int;
	internal static var g_EscapeDown:Boolean;
	public static function g_UpdateKeyboard():void{
		if((bb_input_KeyDown(g_LeftKey))!=0){
			if(g_LeftDown==false){
				g_LeftHit=true;
			}
			g_LeftDown=true;
		}else{
			g_LeftDown=false;
		}
		if((bb_input_KeyDown(g_RightKey))!=0){
			if(g_RightDown==false){
				g_RightHit=true;
			}
			g_RightDown=true;
		}else{
			g_RightDown=false;
		}
		if((bb_input_KeyDown(g_UpKey))!=0){
			if(g_UpDown==false){
				g_UpHit=true;
			}
			g_UpDown=true;
		}else{
			g_UpDown=false;
		}
		if((bb_input_KeyDown(g_DownKey))!=0){
			if(g_DownDown==false){
				g_DownHit=true;
			}
			g_DownDown=true;
		}else{
			g_DownDown=false;
		}
		if((bb_input_KeyDown(g_ActionKey))!=0){
			if(g_ActionDown==false){
				g_ActionHit=true;
			}
			g_ActionDown=true;
		}else{
			g_ActionDown=false;
		}
		if((bb_input_KeyDown(g_Action2Key))!=0){
			if(g_Action2Down==false){
				g_Action2Hit=true;
			}
			g_Action2Down=true;
		}else{
			g_Action2Down=false;
		}
		if((bb_input_KeyDown(g_EscapeKey))!=0){
			if(g_EscapeDown==false){
				g_EscapeHit=true;
			}
			g_EscapeDown=true;
		}else{
			g_EscapeDown=false;
		}
	}
	public static function g_UpdateJoypad():void{
		if(bb_input_JoyX(0,0)<-0.1){
			if(g_LeftDown==false){
				g_LeftHit=true;
			}
			g_LeftDown=true;
		}else{
			g_LeftDown=false;
		}
		if(bb_input_JoyX(0,0)>0.1){
			if(g_RightDown==false){
				g_RightHit=true;
			}
			g_RightDown=true;
		}else{
			g_RightDown=false;
		}
		if(bb_input_JoyY(0,0)>0.1){
			if(g_UpDown==false){
				g_UpHit=true;
			}
			g_UpDown=true;
		}else{
			g_UpDown=false;
		}
		if(bb_input_JoyY(0,0)<-0.1){
			if(g_DownDown==false){
				g_DownHit=true;
			}
			g_DownDown=true;
		}else{
			g_DownDown=false;
		}
		if((bb_input_JoyDown(0,0))!=0){
			if(g_ActionDown==false){
				g_ActionHit=true;
			}
			g_ActionDown=true;
		}else{
			g_ActionDown=false;
		}
		if((bb_input_JoyDown(1,0))!=0){
			if(g_Action2Down==false){
				g_Action2Hit=true;
			}
			g_Action2Down=true;
		}else{
			g_Action2Down=false;
		}
		if((bb_input_JoyDown(7,0))!=0){
			if(g_EscapeDown==false){
				g_EscapeHit=true;
			}
			g_EscapeDown=true;
		}else{
			g_EscapeDown=false;
		}
	}
	internal static var g_TouchPoint:Boolean;
	public static function g_UpdateTouch():void{
		g_TCAction1.f_Hit=false;
		g_TCAction2.f_Hit=false;
		g_TCEscapeKey.f_Hit=false;
		if((bb_input_MouseHit(0))!=0){
			g_TouchPoint=true;
			g_TCMove.m_StartTouch(bb_autofit_VMouseX(true),bb_autofit_VMouseY(true),0);
			for(var t_i:int=0;t_i<=2;t_i=t_i+1){
				if(g_TCButtons[t_i].m_Check(((bb_autofit_VMouseX(true))|0),((bb_autofit_VMouseY(true))|0))){
					g_TCButtons[t_i].f_Hit=true;
				}
			}
		}else{
			if((bb_input_MouseDown(0))!=0){
				g_TCMove.m_UpdateTouch(bb_autofit_VMouseX(true),bb_autofit_VMouseY(true),0);
				for(var t_i2:int=0;t_i2<=2;t_i2=t_i2+1){
					if(g_TCButtons[t_i2].m_Check(((bb_autofit_VMouseX(true))|0),((bb_autofit_VMouseY(true))|0))){
						g_TCButtons[t_i2].f_Down=true;
					}
				}
			}else{
				if(g_TouchPoint){
					g_TouchPoint=false;
					g_TCMove.m_StopTouch(0);
					for(var t_i3:int=0;t_i3<=2;t_i3=t_i3+1){
						g_TCButtons[t_i3].f_Down=false;
					}
				}
			}
		}
		if(g_TCMove.m_GetDX()<-0.1){
			if(g_LeftDown==false){
				g_LeftHit=true;
			}
			g_LeftDown=true;
		}else{
			g_LeftDown=false;
		}
		if(g_TCMove.m_GetDX()>0.1){
			if(g_RightDown==false){
				g_RightHit=true;
			}
			g_RightDown=true;
		}else{
			g_RightDown=false;
		}
		if(g_TCMove.m_GetDY()>0.1){
			if(g_UpDown==false){
				g_UpHit=true;
			}
			g_UpDown=true;
		}else{
			g_UpDown=false;
		}
		if(g_TCMove.m_GetDY()<-0.1){
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
	public static function g_Update():void{
		g_LeftHit=false;
		g_RightHit=false;
		g_DownHit=false;
		g_UpHit=false;
		g_ActionHit=false;
		g_Action2Hit=false;
		g_EscapeHit=false;
		var t_:int=g_ControlMethod;
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
class bb_virtualstick_VirtualStick extends Object{
	public function g_VirtualStick_new():bb_virtualstick_VirtualStick{
		return this;
	}
	internal var f_ringX:Number=.0;
	internal var f_ringY:Number=.0;
	internal var f_ringRadius:Number=.0;
	public function m_SetRing(t_ringX:Number,t_ringY:Number,t_ringRadius:Number):void{
		this.f_ringX=t_ringX;
		this.f_ringY=t_ringY;
		this.f_ringRadius=t_ringRadius;
	}
	internal var f_stickX:Number=0.0;
	internal var f_stickY:Number=0.0;
	internal var f_stickRadius:Number=.0;
	public function m_SetStick(t_stickX:Number,t_stickY:Number,t_stickRadius:Number):void{
		this.f_stickX=t_stickX;
		this.f_stickY=t_stickY;
		this.f_stickRadius=t_stickRadius;
	}
	internal var f_deadZone:Number=.0;
	public function m_SetDeadZone(t_deadZone:Number):void{
		this.f_deadZone=t_deadZone;
	}
	internal var f_triggerDistance:Number=-1.0;
	public function m_SetTriggerDistance(t_triggerDistance:Number):void{
		this.f_triggerDistance=t_triggerDistance;
	}
	internal var f_touchNumber:int=-1;
	internal var f_firstTouchX:Number=.0;
	internal var f_firstTouchY:Number=.0;
	internal var f_triggered:Boolean=false;
	internal var f_stickPower:Number=.0;
	internal var f_stickAngle:Number=.0;
	public function m_UpdateStick():void{
		if(this.f_touchNumber>=0){
			var t_length:Number=Math.sqrt(this.f_stickX*this.f_stickX+this.f_stickY*this.f_stickY);
			this.f_stickPower=t_length/this.f_ringRadius;
			if(this.f_stickPower>1.0){
				this.f_stickPower=1.0;
			}
			if(this.f_stickPower<this.f_deadZone){
				this.f_stickPower=0.0;
				this.f_stickAngle=0.0;
				this.f_stickX=0.0;
				this.f_stickY=0.0;
			}else{
				if(this.f_stickX==0.0 && this.f_stickY==0.0){
					this.f_stickAngle=0.0;
					this.f_stickPower=0.0;
				}else{
					if(this.f_stickX==0.0 && this.f_stickY>0.0){
						this.f_stickAngle=90.0;
					}else{
						if(this.f_stickX==0.0 && this.f_stickY<0.0){
							this.f_stickAngle=270.0;
						}else{
							if(this.f_stickY==0.0 && this.f_stickX>0.0){
								this.f_stickAngle=0.0;
							}else{
								if(this.f_stickY==0.0 && this.f_stickX<0.0){
									this.f_stickAngle=180.0;
								}else{
									if(this.f_stickX>0.0 && this.f_stickY>0.0){
										this.f_stickAngle=(Math.atan(this.f_stickY/this.f_stickX)*R2D);
									}else{
										if(this.f_stickX<0.0){
											this.f_stickAngle=180.0+(Math.atan(this.f_stickY/this.f_stickX)*R2D);
										}else{
											this.f_stickAngle=360.0+(Math.atan(this.f_stickY/this.f_stickX)*R2D);
										}
									}
								}
							}
						}
					}
				}
				if(t_length>this.f_ringRadius){
					this.f_stickPower=1.0;
					this.f_stickX=Math.cos((this.f_stickAngle)*D2R)*this.f_ringRadius;
					this.f_stickY=Math.sin((this.f_stickAngle)*D2R)*this.f_ringRadius;
				}
			}
		}
	}
	public function m_StartTouch(t_x:Number,t_y:Number,t_touchnum:int):void{
		if(this.f_touchNumber<0){
			if((t_x-this.f_ringX)*(t_x-this.f_ringX)+(t_y-this.f_ringY)*(t_y-this.f_ringY)<=this.f_ringRadius*this.f_ringRadius){
				this.f_touchNumber=t_touchnum;
				this.f_firstTouchX=t_x;
				this.f_firstTouchY=t_y;
				this.f_triggered=false;
				if(this.f_triggerDistance<=0.0){
					this.f_triggered=true;
					this.f_stickX=t_x-this.f_ringX;
					this.f_stickY=this.f_ringY-t_y;
				}
				this.m_UpdateStick();
			}
		}
	}
	public function m_UpdateTouch(t_x:Number,t_y:Number,t_touchnum:int):void{
		if(this.f_touchNumber>=0 && this.f_touchNumber==t_touchnum){
			if(!this.f_triggered){
				if((t_x-this.f_firstTouchX)*(t_x-this.f_firstTouchX)+(t_y-this.f_firstTouchY)*(t_y-this.f_firstTouchY)>this.f_triggerDistance*this.f_triggerDistance){
					this.f_triggered=true;
				}
			}
			if(this.f_triggered){
				this.f_stickX=t_x-this.f_ringX;
				this.f_stickY=this.f_ringY-t_y;
				this.m_UpdateStick();
			}
		}
	}
	public function m_StopTouch(t_touchnum:int):void{
		if(this.f_touchNumber>=0 && this.f_touchNumber==t_touchnum){
			this.f_touchNumber=-1;
			this.f_stickX=0.0;
			this.f_stickY=0.0;
			this.f_stickAngle=0.0;
			this.f_stickPower=0.0;
			this.f_triggered=false;
		}
	}
	public function m_GetDX():Number{
		return Math.cos((this.f_stickAngle)*D2R)*this.f_stickPower;
	}
	public function m_GetDY():Number{
		return Math.sin((this.f_stickAngle)*D2R)*this.f_stickPower;
	}
	public function m_RenderRing(t_x:Number,t_y:Number):void{
		bb_graphics_DrawCircle(t_x,t_y,this.f_ringRadius);
	}
	public function m_DoRenderRing():void{
		this.m_RenderRing(this.f_ringX,this.f_ringY);
	}
	public function m_RenderStick(t_x:Number,t_y:Number):void{
		bb_graphics_DrawCircle(t_x,t_y,this.f_stickRadius);
	}
	public function m_DoRenderStick():void{
		this.m_RenderStick(this.f_ringX+this.f_stickX,this.f_ringY-this.f_stickY);
	}
}
class bb_virtualstick_MyStick extends bb_virtualstick_VirtualStick{
	public function g_MyStick_new():bb_virtualstick_MyStick{
		super.g_VirtualStick_new();
		return this;
	}
	public override function m_RenderRing(t_x:Number,t_y:Number):void{
		bb_graphics_SetColor(0.0,0.0,0.0);
		bb_graphics_SetAlpha(0.1);
		super.m_RenderRing(t_x,t_y);
		bb_graphics_SetColor(255.0,255.0,255.0);
	}
	public override function m_RenderStick(t_x:Number,t_y:Number):void{
		bb_graphics_SetColor(255.0,255.0,255.0);
		bb_graphics_SetAlpha(0.5);
		super.m_RenderStick(t_x,t_y);
		bb_graphics_SetColor(255.0,255.0,255.0);
	}
}
class bb_touchbutton_TouchButton extends Object{
	internal var f_X:int=0;
	internal var f_Y:int=0;
	internal var f_W:int=0;
	internal var f_H:int=0;
	public function g_TouchButton_new(t_tX:int,t_tY:int,t_tW:int,t_tH:int):bb_touchbutton_TouchButton{
		this.f_X=t_tX;
		this.f_Y=t_tY;
		this.f_W=t_tW;
		this.f_H=t_tH;
		return this;
	}
	public function g_TouchButton_new2():bb_touchbutton_TouchButton{
		return this;
	}
	internal var f_Hit:Boolean=false;
	public function m_Check(t_tX:int,t_tY:int):Boolean{
		return bb_functions_PointInRect((t_tX),(t_tY),(this.f_X),(this.f_Y),(this.f_W),(this.f_H));
	}
	internal var f_Down:Boolean=false;
	public function m_Render():void{
		bb_graphics_SetColor(0.0,0.0,0.0);
		bb_graphics_SetAlpha(0.25);
		bb_graphics_DrawRect((this.f_X),(this.f_Y),(this.f_W),(this.f_H));
		bb_graphics_SetColor(255.0,255.0,255.0);
		bb_graphics_SetAlpha(0.5);
		bb_gfx_DrawHollowRect(this.f_X,this.f_Y,this.f_W,this.f_H);
	}
}
class bb_raztext_RazText extends Object{
	internal static var g_TextSheet:bb_graphics_Image;
	public static function g_SetTextSheet(t_tImage:bb_graphics_Image):void{
		g_TextSheet=t_tImage;
	}
	internal var f_Lines:bb_list_List=null;
	public function g_RazText_new():bb_raztext_RazText{
		this.f_Lines=(new bb_list_List).g_List_new();
		return this;
	}
	internal var f_OriginalString:String="";
	public function m_AddLine(t_tString:String):void{
		var t_tmp:Array=new_object_array(t_tString.length);
		for(var t_i:int=0;t_i<=t_tString.length-1;t_i=t_i+1){
			var t_let:int=t_tString.charCodeAt(t_i);
			var t_letstring:String=t_tString.slice(t_i,t_i+1).toLowerCase();
			var t_XOff:int=0;
			var t_YOff:int=0;
			var t_:String=t_letstring.toLowerCase();
			if(t_=="a"){
				t_XOff=0;
				t_YOff=1;
			}else{
				if(t_=="b"){
					t_XOff=1;
					t_YOff=1;
				}else{
					if(t_=="c"){
						t_XOff=2;
						t_YOff=1;
					}else{
						if(t_=="d"){
							t_XOff=3;
							t_YOff=1;
						}else{
							if(t_=="e"){
								t_XOff=4;
								t_YOff=1;
							}else{
								if(t_=="f"){
									t_XOff=5;
									t_YOff=1;
								}else{
									if(t_=="g"){
										t_XOff=6;
										t_YOff=1;
									}else{
										if(t_=="h"){
											t_XOff=7;
											t_YOff=1;
										}else{
											if(t_=="i"){
												t_XOff=8;
												t_YOff=1;
											}else{
												if(t_=="j"){
													t_XOff=9;
													t_YOff=1;
												}else{
													if(t_=="k"){
														t_XOff=0;
														t_YOff=2;
													}else{
														if(t_=="l"){
															t_XOff=1;
															t_YOff=2;
														}else{
															if(t_=="m"){
																t_XOff=2;
																t_YOff=2;
															}else{
																if(t_=="n"){
																	t_XOff=3;
																	t_YOff=2;
																}else{
																	if(t_=="o"){
																		t_XOff=4;
																		t_YOff=2;
																	}else{
																		if(t_=="p"){
																			t_XOff=5;
																			t_YOff=2;
																		}else{
																			if(t_=="q"){
																				t_XOff=6;
																				t_YOff=2;
																			}else{
																				if(t_=="r"){
																					t_XOff=7;
																					t_YOff=2;
																				}else{
																					if(t_=="s"){
																						t_XOff=8;
																						t_YOff=2;
																					}else{
																						if(t_=="t"){
																							t_XOff=9;
																							t_YOff=2;
																						}else{
																							if(t_=="u"){
																								t_XOff=0;
																								t_YOff=3;
																							}else{
																								if(t_=="v"){
																									t_XOff=1;
																									t_YOff=3;
																								}else{
																									if(t_=="w"){
																										t_XOff=2;
																										t_YOff=3;
																									}else{
																										if(t_=="x"){
																											t_XOff=3;
																											t_YOff=3;
																										}else{
																											if(t_=="y"){
																												t_XOff=4;
																												t_YOff=3;
																											}else{
																												if(t_=="z"){
																													t_XOff=5;
																													t_YOff=3;
																												}else{
																													if(t_=="0"){
																														t_XOff=9;
																														t_YOff=0;
																													}else{
																														if(t_=="1"){
																															t_XOff=0;
																															t_YOff=0;
																														}else{
																															if(t_=="2"){
																																t_XOff=1;
																																t_YOff=0;
																															}else{
																																if(t_=="3"){
																																	t_XOff=2;
																																	t_YOff=0;
																																}else{
																																	if(t_=="4"){
																																		t_XOff=3;
																																		t_YOff=0;
																																	}else{
																																		if(t_=="5"){
																																			t_XOff=4;
																																			t_YOff=0;
																																		}else{
																																			if(t_=="6"){
																																				t_XOff=5;
																																				t_YOff=0;
																																			}else{
																																				if(t_=="7"){
																																					t_XOff=6;
																																					t_YOff=0;
																																				}else{
																																					if(t_=="8"){
																																						t_XOff=7;
																																						t_YOff=0;
																																					}else{
																																						if(t_=="9"){
																																							t_XOff=8;
																																							t_YOff=0;
																																						}else{
																																							if(t_==","){
																																								t_XOff=6;
																																								t_YOff=3;
																																							}else{
																																								if(t_=="."){
																																									t_XOff=7;
																																									t_YOff=3;
																																								}else{
																																									if(t_=="!"){
																																										t_XOff=8;
																																										t_YOff=3;
																																									}else{
																																										if(t_=="?"){
																																											t_XOff=9;
																																											t_YOff=3;
																																										}else{
																																											if(t_=="@"){
																																												t_XOff=1;
																																												t_YOff=4;
																																											}else{
																																												if(t_==" "){
																																													t_XOff=9;
																																													t_YOff=4;
																																												}else{
																																													if(t_=="/"){
																																														t_XOff=0;
																																														t_YOff=4;
																																													}else{
																																														if(t_=="-"){
																																															t_XOff=2;
																																															t_YOff=4;
																																														}else{
																																															if(t_==":"){
																																																t_XOff=3;
																																																t_YOff=4;
																																															}else{
																																																if(t_==";"){
																																																	t_XOff=4;
																																																	t_YOff=4;
																																																}else{
																																																	if(t_=="_"){
																																																		t_XOff=5;
																																																		t_YOff=4;
																																																	}else{
																																																		if(t_=="("){
																																																			t_XOff=6;
																																																			t_YOff=4;
																																																		}else{
																																																			if(t_==")"){
																																																				t_XOff=7;
																																																				t_YOff=4;
																																																			}else{
																																																				if(t_=="*"){
																																																					t_XOff=8;
																																																					t_YOff=4;
																																																				}else{
																																																					if(t_=="+"){
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
			t_tmp[t_i]=(new bb_raztext_RazChar).g_RazChar_new();
			t_tmp[t_i].f_XOff=t_XOff;
			t_tmp[t_i].f_YOff=t_YOff;
			t_tmp[t_i].f_TextValue=t_letstring;
			if(t_XOff==9 && t_YOff==4){
				t_tmp[t_i].f_Visible=false;
			}
		}
		this.f_Lines.m_AddLast(t_tmp);
	}
	public function g_RazText_new2(t_tString:String):bb_raztext_RazText{
		this.f_Lines=(new bb_list_List).g_List_new();
		this.f_OriginalString=t_tString;
		this.m_AddLine(t_tString);
		return this;
	}
	public function m_AddMutliLines(t_tString:String):void{
		var t_tmp:Array=t_tString.split("\r");
		for(var t_i:int=0;t_i<=t_tmp.length-1;t_i=t_i+1){
			this.m_AddLine(t_tmp[t_i]);
		}
	}
	internal var f_X:int=0;
	internal var f_Y:int=0;
	public function m_SetPos(t_tX:int,t_tY:int):void{
		this.f_X=t_tX;
		this.f_Y=t_tY;
	}
	internal var f_VerticalSpacing:int=0;
	internal var f_HorizontalSpacing:int=-2;
	public function m_SetSpacing(t_tX:int,t_tY:int):void{
		this.f_VerticalSpacing=t_tY;
		this.f_HorizontalSpacing=t_tX;
	}
}
class bb_gamescreen_GameScreen extends bb_screen_Screen{
	public function g_GameScreen_new():bb_gamescreen_GameScreen{
		super.g_Screen_new();
		return this;
	}
	internal var f_level:bb_level_Level=null;
	public override function m_OnScreenStart():void{
		bb_ld_LDApp.g_level=bb_level_GenerateLevel();
		this.f_level=bb_ld_LDApp.g_level;
		this.f_level.m_Start();
	}
	public override function m_OnScreenEnd():void{
	}
	public override function m_Update():void{
		if(this.f_level!=null){
			this.f_level.m_Update();
		}
	}
	public override function m_Render():void{
		bb_graphics_Cls(255.0,255.0,255.0);
		if(this.f_level!=null){
			this.f_level.m_Render();
		}
	}
}
class bb_map_Node extends Object{
	internal var f_key:String="";
	internal var f_right:bb_map_Node=null;
	internal var f_left:bb_map_Node=null;
	internal var f_value:bb_screen_Screen=null;
	internal var f_color:int=0;
	internal var f_parent:bb_map_Node=null;
	public function g_Node_new(t_key:String,t_value:bb_screen_Screen,t_color:int,t_parent:bb_map_Node):bb_map_Node{
		this.f_key=t_key;
		this.f_value=t_value;
		this.f_color=t_color;
		this.f_parent=t_parent;
		return this;
	}
	public function g_Node_new2():bb_map_Node{
		return this;
	}
}
class bb_titlescreen_TitleScreen extends bb_screen_Screen{
	public function g_TitleScreen_new():bb_titlescreen_TitleScreen{
		super.g_Screen_new();
		return this;
	}
	public override function m_OnScreenStart():void{
	}
	public override function m_OnScreenEnd():void{
	}
	public override function m_Update():void{
		bb_random_Rnd();
		if(bb_controls_Controls.g_ActionHit){
			bb_screenmanager_ScreenManager.g_ChangeScreen("game");
		}
	}
	public override function m_Render():void{
	}
}
class bb_map_Node2 extends Object{
	internal var f_key:String="";
	internal var f_right:bb_map_Node2=null;
	internal var f_left:bb_map_Node2=null;
	internal var f_value:String="";
	internal var f_color:int=0;
	internal var f_parent:bb_map_Node2=null;
	public function g_Node_new(t_key:String,t_value:String,t_color:int,t_parent:bb_map_Node2):bb_map_Node2{
		this.f_key=t_key;
		this.f_value=t_value;
		this.f_color=t_color;
		this.f_parent=t_parent;
		return this;
	}
	public function g_Node_new2():bb_map_Node2{
		return this;
	}
}
internal function bb_math_Clamp(t_n:int,t_min:int,t_max:int):int{
	if(t_n<t_min){
		return t_min;
	}
	if(t_n>t_max){
		return t_max;
	}
	return t_n;
}
internal function bb_math_Clamp2(t_n:Number,t_min:Number,t_max:Number):Number{
	if(t_n<t_min){
		return t_min;
	}
	if(t_n>t_max){
		return t_max;
	}
	return t_n;
}
class bb_controls_ControlMethodTypes extends Object{
}
internal function bb_app_SetUpdateRate(t_hertz:int):int{
	return bb_app_device.SetUpdateRate(t_hertz);
}
class bb_autofit_VirtualDisplay extends Object{
	internal var f_vwidth:Number=.0;
	internal var f_vheight:Number=.0;
	internal var f_vzoom:Number=.0;
	internal var f_lastvzoom:Number=.0;
	internal var f_vratio:Number=.0;
	internal static var g_Display:bb_autofit_VirtualDisplay;
	public function g_VirtualDisplay_new(t_width:int,t_height:int,t_zoom:Number):bb_autofit_VirtualDisplay{
		this.f_vwidth=(t_width);
		this.f_vheight=(t_height);
		this.f_vzoom=t_zoom;
		this.f_lastvzoom=this.f_vzoom+1.0;
		this.f_vratio=this.f_vheight/this.f_vwidth;
		g_Display=this;
		return this;
	}
	public function g_VirtualDisplay_new2():bb_autofit_VirtualDisplay{
		return this;
	}
	internal var f_multi:Number=.0;
	public function m_VMouseX(t_limit:Boolean):Number{
		var t_mouseoffset:Number=bb_input_MouseX()-(bb_graphics_DeviceWidth())*0.5;
		var t_x:Number=t_mouseoffset/this.f_multi/this.f_vzoom+bb_autofit_VDeviceWidth()*0.5;
		if(t_limit){
			var t_widthlimit:Number=this.f_vwidth-1.0;
			if(t_x>0.0){
				if(t_x<t_widthlimit){
					return t_x;
				}else{
					return t_widthlimit;
				}
			}else{
				return 0.0;
			}
		}else{
			return t_x;
		}
	}
	public function m_VMouseY(t_limit:Boolean):Number{
		var t_mouseoffset:Number=bb_input_MouseY()-(bb_graphics_DeviceHeight())*0.5;
		var t_y:Number=t_mouseoffset/this.f_multi/this.f_vzoom+bb_autofit_VDeviceHeight()*0.5;
		if(t_limit){
			var t_heightlimit:Number=this.f_vheight-1.0;
			if(t_y>0.0){
				if(t_y<t_heightlimit){
					return t_y;
				}else{
					return t_heightlimit;
				}
			}else{
				return 0.0;
			}
		}else{
			return t_y;
		}
	}
	internal var f_lastdevicewidth:int=0;
	internal var f_lastdeviceheight:int=0;
	internal var f_device_changed:int=0;
	internal var f_fdw:Number=.0;
	internal var f_fdh:Number=.0;
	internal var f_dratio:Number=.0;
	internal var f_heightborder:Number=.0;
	internal var f_widthborder:Number=.0;
	internal var f_zoom_changed:int=0;
	internal var f_realx:Number=.0;
	internal var f_realy:Number=.0;
	internal var f_offx:Number=.0;
	internal var f_offy:Number=.0;
	internal var f_sx:Number=.0;
	internal var f_sw:Number=.0;
	internal var f_sy:Number=.0;
	internal var f_sh:Number=.0;
	internal var f_scaledw:Number=.0;
	internal var f_scaledh:Number=.0;
	internal var f_vxoff:Number=.0;
	internal var f_vyoff:Number=.0;
	public function m_UpdateVirtualDisplay(t_zoomborders:Boolean,t_keepborders:Boolean):int{
		if(bb_graphics_DeviceWidth()!=this.f_lastdevicewidth || bb_graphics_DeviceHeight()!=this.f_lastdeviceheight){
			this.f_lastdevicewidth=bb_graphics_DeviceWidth();
			this.f_lastdeviceheight=bb_graphics_DeviceHeight();
			this.f_device_changed=1;
		}
		if((this.f_device_changed)!=0){
			this.f_fdw=(bb_graphics_DeviceWidth());
			this.f_fdh=(bb_graphics_DeviceHeight());
			this.f_dratio=this.f_fdh/this.f_fdw;
			if(this.f_dratio>this.f_vratio){
				this.f_multi=this.f_fdw/this.f_vwidth;
				this.f_heightborder=(this.f_fdh-this.f_vheight*this.f_multi)*0.5;
				this.f_widthborder=0.0;
			}else{
				this.f_multi=this.f_fdh/this.f_vheight;
				this.f_widthborder=(this.f_fdw-this.f_vwidth*this.f_multi)*0.5;
				this.f_heightborder=0.0;
			}
		}
		if(this.f_vzoom!=this.f_lastvzoom){
			this.f_lastvzoom=this.f_vzoom;
			this.f_zoom_changed=1;
		}
		if(((this.f_zoom_changed)!=0) || ((this.f_device_changed)!=0)){
			if(t_zoomborders){
				this.f_realx=this.f_vwidth*this.f_vzoom*this.f_multi;
				this.f_realy=this.f_vheight*this.f_vzoom*this.f_multi;
				this.f_offx=(this.f_fdw-this.f_realx)*0.5;
				this.f_offy=(this.f_fdh-this.f_realy)*0.5;
				if(t_keepborders){
					if(this.f_offx<this.f_widthborder){
						this.f_sx=this.f_widthborder;
						this.f_sw=this.f_fdw-this.f_widthborder*2.0;
					}else{
						this.f_sx=this.f_offx;
						this.f_sw=this.f_fdw-this.f_offx*2.0;
					}
					if(this.f_offy<this.f_heightborder){
						this.f_sy=this.f_heightborder;
						this.f_sh=this.f_fdh-this.f_heightborder*2.0;
					}else{
						this.f_sy=this.f_offy;
						this.f_sh=this.f_fdh-this.f_offy*2.0;
					}
				}else{
					this.f_sx=this.f_offx;
					this.f_sw=this.f_fdw-this.f_offx*2.0;
					this.f_sy=this.f_offy;
					this.f_sh=this.f_fdh-this.f_offy*2.0;
				}
				this.f_sx=bb_math_Max2(0.0,this.f_sx);
				this.f_sy=bb_math_Max2(0.0,this.f_sy);
				this.f_sw=bb_math_Min2(this.f_sw,this.f_fdw);
				this.f_sh=bb_math_Min2(this.f_sh,this.f_fdh);
			}else{
				this.f_sx=bb_math_Max2(0.0,this.f_widthborder);
				this.f_sy=bb_math_Max2(0.0,this.f_heightborder);
				this.f_sw=bb_math_Min2(this.f_fdw-this.f_widthborder*2.0,this.f_fdw);
				this.f_sh=bb_math_Min2(this.f_fdh-this.f_heightborder*2.0,this.f_fdh);
			}
			this.f_scaledw=this.f_vwidth*this.f_multi*this.f_vzoom;
			this.f_scaledh=this.f_vheight*this.f_multi*this.f_vzoom;
			this.f_vxoff=(this.f_fdw-this.f_scaledw)*0.5;
			this.f_vyoff=(this.f_fdh-this.f_scaledh)*0.5;
			this.f_vxoff=this.f_vxoff/this.f_multi/this.f_vzoom;
			this.f_vyoff=this.f_vyoff/this.f_multi/this.f_vzoom;
			this.f_device_changed=0;
			this.f_zoom_changed=0;
		}
		bb_graphics_SetScissor(0.0,0.0,(bb_graphics_DeviceWidth()),(bb_graphics_DeviceHeight()));
		bb_graphics_Cls(0.0,0.0,0.0);
		bb_graphics_SetScissor(this.f_sx,this.f_sy,this.f_sw,this.f_sh);
		bb_graphics_Scale(this.f_multi*this.f_vzoom,this.f_multi*this.f_vzoom);
		if((this.f_vzoom)!=0.0){
			bb_graphics_Translate(this.f_vxoff,this.f_vyoff);
		}
		return 0;
	}
}
internal function bb_autofit_SetVirtualDisplay(t_width:int,t_height:int,t_zoom:Number):int{
	(new bb_autofit_VirtualDisplay).g_VirtualDisplay_new(t_width,t_height,t_zoom);
	return 0;
}
internal function bb_input_KeyDown(t_key:int):int{
	return bb_input_device.KeyDown(t_key);
}
internal function bb_input_JoyX(t_index:int,t_unit:int):Number{
	return bb_input_device.JoyX(t_index|t_unit<<4);
}
internal function bb_input_JoyY(t_index:int,t_unit:int):Number{
	return bb_input_device.JoyY(t_index|t_unit<<4);
}
internal function bb_input_JoyDown(t_button:int,t_unit:int):int{
	return bb_input_device.KeyDown(t_button|t_unit<<4|256);
}
internal function bb_input_MouseHit(t_button:int):int{
	return bb_input_device.KeyHit(1+t_button);
}
internal function bb_input_MouseX():Number{
	return bb_input_device.MouseX();
}
internal function bb_autofit_VDeviceWidth():Number{
	return bb_autofit_VirtualDisplay.g_Display.f_vwidth;
}
internal function bb_autofit_VMouseX(t_limit:Boolean):Number{
	return bb_autofit_VirtualDisplay.g_Display.m_VMouseX(t_limit);
}
internal function bb_input_MouseY():Number{
	return bb_input_device.MouseY();
}
internal function bb_autofit_VDeviceHeight():Number{
	return bb_autofit_VirtualDisplay.g_Display.f_vheight;
}
internal function bb_autofit_VMouseY(t_limit:Boolean):Number{
	return bb_autofit_VirtualDisplay.g_Display.m_VMouseY(t_limit);
}
internal function bb_functions_PointInRect(t_X:Number,t_Y:Number,t_X1:Number,t_Y1:Number,t_W:Number,t_H:Number):Boolean{
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
internal function bb_input_MouseDown(t_button:int):int{
	return bb_input_device.KeyDown(1+t_button);
}
internal function bb_math_Max(t_x:int,t_y:int):int{
	if(t_x>t_y){
		return t_x;
	}
	return t_y;
}
internal function bb_math_Max2(t_x:Number,t_y:Number):Number{
	if(t_x>t_y){
		return t_x;
	}
	return t_y;
}
internal function bb_math_Min(t_x:int,t_y:int):int{
	if(t_x<t_y){
		return t_x;
	}
	return t_y;
}
internal function bb_math_Min2(t_x:Number,t_y:Number):Number{
	if(t_x<t_y){
		return t_x;
	}
	return t_y;
}
internal function bb_graphics_Cls(t_r:Number,t_g:Number,t_b:Number):int{
	bb_graphics_renderDevice.Cls(t_r,t_g,t_b);
	return 0;
}
internal function bb_graphics_Transform(t_ix:Number,t_iy:Number,t_jx:Number,t_jy:Number,t_tx:Number,t_ty:Number):int{
	var t_ix2:Number=t_ix*bb_graphics_context.f_ix+t_iy*bb_graphics_context.f_jx;
	var t_iy2:Number=t_ix*bb_graphics_context.f_iy+t_iy*bb_graphics_context.f_jy;
	var t_jx2:Number=t_jx*bb_graphics_context.f_ix+t_jy*bb_graphics_context.f_jx;
	var t_jy2:Number=t_jx*bb_graphics_context.f_iy+t_jy*bb_graphics_context.f_jy;
	var t_tx2:Number=t_tx*bb_graphics_context.f_ix+t_ty*bb_graphics_context.f_jx+bb_graphics_context.f_tx;
	var t_ty2:Number=t_tx*bb_graphics_context.f_iy+t_ty*bb_graphics_context.f_jy+bb_graphics_context.f_ty;
	bb_graphics_SetMatrix(t_ix2,t_iy2,t_jx2,t_jy2,t_tx2,t_ty2);
	return 0;
}
internal function bb_graphics_Transform2(t_m:Array):int{
	bb_graphics_Transform(t_m[0],t_m[1],t_m[2],t_m[3],t_m[4],t_m[5]);
	return 0;
}
internal function bb_graphics_Scale(t_x:Number,t_y:Number):int{
	bb_graphics_Transform(t_x,0.0,0.0,t_y,0.0,0.0);
	return 0;
}
internal function bb_graphics_Translate(t_x:Number,t_y:Number):int{
	bb_graphics_Transform(1.0,0.0,0.0,1.0,t_x,t_y);
	return 0;
}
internal function bb_autofit_UpdateVirtualDisplay(t_zoomborders:Boolean,t_keepborders:Boolean):int{
	bb_autofit_VirtualDisplay.g_Display.m_UpdateVirtualDisplay(t_zoomborders,t_keepborders);
	return 0;
}
internal function bb_graphics_DrawCircle(t_x:Number,t_y:Number,t_r:Number):int{
	bb_graphics_context.m_Validate();
	bb_graphics_renderDevice.DrawOval(t_x-t_r,t_y-t_r,t_r*2.0,t_r*2.0);
	return 0;
}
internal function bb_graphics_DrawRect(t_x:Number,t_y:Number,t_w:Number,t_h:Number):int{
	bb_graphics_context.m_Validate();
	bb_graphics_renderDevice.DrawRect(t_x,t_y,t_w,t_h);
	return 0;
}
internal function bb_graphics_DrawLine(t_x1:Number,t_y1:Number,t_x2:Number,t_y2:Number):int{
	bb_graphics_context.m_Validate();
	bb_graphics_renderDevice.DrawLine(t_x1,t_y1,t_x2,t_y2);
	return 0;
}
internal function bb_gfx_DrawHollowRect(t_tX:int,t_tY:int,t_tW:int,t_tH:int):void{
	var t_X1:int=t_tX;
	var t_Y1:int=t_tY;
	var t_X2:int=t_tX+t_tW;
	var t_Y2:int=t_tY+t_tH;
	bb_graphics_DrawLine((t_X1),(t_Y1),(t_X2),(t_Y1));
	bb_graphics_DrawLine((t_X1),(t_Y2),(t_X2),(t_Y2));
	bb_graphics_DrawLine((t_X1),(t_Y1),(t_X1),(t_Y2));
	bb_graphics_DrawLine((t_X2),(t_Y1),(t_X2),(t_Y2));
}
class bb_level_Level extends Object{
	internal var f_controlledYeti:int=0;
	internal var f_txtWait:bb_raztext_RazText=null;
	internal var f_titleFade:Number=0.0;
	internal var f_titleFadeMode:int=0;
	internal var f_titleFadeTimer:int=0;
	public function g_Level_new():bb_level_Level{
		bb_entity_Entity.g_Init();
		bb_dog_Dog.g_Init(bb_ld_LDApp.g_level);
		bb_yeti_Yeti.g_Init(bb_ld_LDApp.g_level);
		bb_skier_Skier.g_Init(bb_ld_LDApp.g_level);
		bb_tree_Tree.g_Init(bb_ld_LDApp.g_level);
		bb_rock_Rock.g_Init(bb_ld_LDApp.g_level);
		bb_flag_Flag.g_Init(bb_ld_LDApp.g_level);
		this.f_controlledYeti=bb_yeti_Yeti.g_Create(0.0,0.0);
		bb_yeti_Yeti.g_a[this.f_controlledYeti].m_StartWaiting();
		bb_dog_Dog.g_Create(50.0,50.0);
		var t_firstSkier:int=bb_skier_Skier.g_Create(50.0,-200.0);
		bb_skier_Skier.g_a[t_firstSkier].f_TargetYeti=0;
		bb_skier_Skier.g_a[t_firstSkier].m_StartPreTeasing();
		bb_ld_LDApp.g_SetScreenPosition(bb_yeti_Yeti.g_a[this.f_controlledYeti].f_X,bb_yeti_Yeti.g_a[this.f_controlledYeti].f_Y);
		this.f_txtWait=(new bb_raztext_RazText).g_RazText_new();
		this.f_txtWait.m_AddMutliLines(string_replace(bb_app_LoadString("txt/wait.txt"),"\n",""));
		this.f_txtWait.m_SetPos(96,320);
		this.f_txtWait.m_SetSpacing(-3,-1);
		this.f_titleFade=0.0;
		this.f_titleFadeMode=0;
		this.f_titleFadeTimer=0;
		return this;
	}
	internal static var g_Width:int;
	internal static var g_Height:int;
	public function m_Start():void{
		bb_sfx_SFX.g_Music("ambient",1);
	}
	public function m_UpdateTitleFade():void{
		var t_:int=this.f_titleFadeMode;
		if(t_==0){
			this.f_titleFadeTimer+=1;
			if(this.f_titleFadeTimer>=120){
				this.f_titleFadeTimer=0;
				this.f_titleFadeMode=1;
			}
		}else{
			if(t_==1){
				this.f_titleFade+=0.02;
				if(this.f_titleFade>=1.0){
					this.f_titleFade=1.0;
					this.f_titleFadeMode=2;
				}
			}else{
				if(t_==2){
					this.f_titleFadeTimer+=1;
					if(this.f_titleFadeTimer>=240){
						this.f_titleFadeTimer=0;
						this.f_titleFadeMode=3;
					}
				}else{
					if(t_==3){
						this.f_titleFade-=0.01;
						if(this.f_titleFade<=0.0){
							this.f_titleFade=0.0;
							this.f_titleFadeMode=4;
						}
					}
				}
			}
		}
	}
	public function m_Update():void{
		bb_ld_LDApp.g_SetScreenTarget(bb_yeti_Yeti.g_a[this.f_controlledYeti].f_X,bb_yeti_Yeti.g_a[this.f_controlledYeti].f_Y+(bb_ld_LDApp.g_ScreenHeight)*0.15);
		bb_dog_Dog.g_UpdateAll();
		bb_yeti_Yeti.g_UpdateAll();
		bb_skier_Skier.g_UpdateAll();
		bb_tree_Tree.g_UpdateAll();
		bb_rock_Rock.g_UpdateAll();
		this.m_UpdateTitleFade();
	}
	public function m_RenderGui():void{
		bb_graphics_SetColor(255.0,255.0,255.0);
		bb_graphics_SetAlpha(1.0);
		bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0,1.0,504,0,8,360,0);
		bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,6.0,1.0+bb_yeti_Yeti.g_a[this.f_controlledYeti].f_Y/50.0,464,0,10,10,0);
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			if(bb_skier_Skier.g_a[t_i].f_Active==true){
				bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,6.0,1.0+bb_skier_Skier.g_a[t_i].f_Y/50.0,480,0,10,10,0);
			}
		}
	}
	public function m_RenderTitleFade():void{
		if(this.f_titleFadeMode>=1 && this.f_titleFadeMode<4){
			bb_graphics_SetAlpha(this.f_titleFade);
			bb_graphics_DrawImage(bb_gfx_GFX.g_Title,(bb_ld_LDApp.g_ScreenWidth)*0.5,(bb_ld_LDApp.g_ScreenHeight)*0.5,0);
			bb_graphics_SetAlpha(1.0);
		}
	}
	public function m_Render():void{
		bb_graphics_SetColor(0.0,0.0,0.0);
		bb_graphics_SetAlpha(0.5);
		bb_graphics_DrawImage(bb_gfx_GFX.g_Overlay,0.0,0.0,0);
		bb_graphics_SetColor(255.0,255.0,255.0);
		bb_graphics_SetAlpha(1.0);
		this.m_RenderGui();
		bb_dog_Dog.g_RenderAll();
		bb_yeti_Yeti.g_RenderAll();
		bb_skier_Skier.g_RenderAll();
		bb_tree_Tree.g_RenderAll();
		bb_rock_Rock.g_RenderAll();
		bb_flag_Flag.g_RenderAll();
		this.m_RenderTitleFade();
	}
}
class bb_entity_Entity extends Object{
	internal static var g_a:Array;
	public static function g_Init():void{
		g_a=new_array_array(9);
		for(var t_i:int=0;t_i<9;t_i=t_i+1){
		}
	}
	internal var f_level:bb_level_Level=null;
	internal var f_W:Number=.0;
	internal var f_H:Number=.0;
	public function g_Entity_new(t_tLev:bb_level_Level):bb_entity_Entity{
		this.f_level=t_tLev;
		this.f_W=16.0;
		this.f_H=16.0;
		return this;
	}
	public function g_Entity_new2():bb_entity_Entity{
		return this;
	}
	internal var f_EnType:int=0;
	internal var f_Active:Boolean=false;
	public function m_Activate():void{
		this.f_Active=true;
	}
	internal var f_X:Number=.0;
	internal var f_Y:Number=.0;
	public function m_SetPosition(t_tX:Number,t_tY:Number):void{
		this.f_X=t_tX;
		this.f_Y=t_tY;
	}
	internal var f_YS:Number=.0;
	internal var f_Z:Number=.0;
	internal var f_onFloor:Boolean=false;
	public function m_Update():void{
		if(this.f_Z>=0.0){
			this.f_onFloor=true;
			this.f_Z=0.0;
		}else{
			this.f_onFloor=false;
		}
	}
	public function m_IsOnScreen(t_tAdditionalBuffer:int):Boolean{
		return bb_functions_RectOverRect(this.f_X,this.f_Y,this.f_W,this.f_H,(bb_ld_LDApp.g_ScreenX-50-t_tAdditionalBuffer),(bb_ld_LDApp.g_ScreenY-50-t_tAdditionalBuffer),(bb_ld_LDApp.g_ScreenWidth+100+t_tAdditionalBuffer*2),(bb_ld_LDApp.g_ScreenHeight+100+t_tAdditionalBuffer*2));
	}
	public function m_Deactivate():void{
		this.f_Active=false;
	}
	internal var f_XS:Number=.0;
	internal var f_ZS:Number=.0;
	public function m_CheckForCollision():int{
		if(this.f_Z<-2.0){
			return -1;
		}
		for(var t_Type:int=0;t_Type<9;t_Type=t_Type+1){
			if(this.f_EnType!=t_Type){
				var t_l:int=g_a[t_Type].length;
				for(var t_i:int=0;t_i<t_l;t_i=t_i+1){
					if(g_a[t_Type][t_i].f_Active==true){
						if(bb_functions_RectOverRect(this.f_X,this.f_Y,this.f_W,this.f_H,g_a[t_Type][t_i].f_X,g_a[t_Type][t_i].f_Y,g_a[t_Type][t_i].f_W,g_a[t_Type][t_i].f_H)){
							return t_Type;
						}
					}
				}
			}
		}
		return -1;
	}
	public function m_Render():void{
	}
}
class bb_entity_EntityType extends Object{
}
class bb_dog_Dog extends bb_entity_Entity{
	internal static var g_a:Array;
	public function g_Dog_new(t_tLev:bb_level_Level):bb_dog_Dog{
		super.g_Entity_new2();
		this.f_level=t_tLev;
		this.f_EnType=1;
		this.f_W=8.0;
		this.f_H=8.0;
		return this;
	}
	public function g_Dog_new2():bb_dog_Dog{
		super.g_Entity_new2();
		return this;
	}
	internal static var g_gfxStandFront:bb_graphics_Image;
	public static function g_Init(t_tLev:bb_level_Level):void{
		g_a=new_object_array(10);
		bb_entity_Entity.g_a[1]=new_object_array(10);
		for(var t_i:int=0;t_i<10;t_i=t_i+1){
			g_a[t_i]=(new bb_dog_Dog).g_Dog_new(t_tLev);
			bb_entity_Entity.g_a[1][t_i]=(g_a[t_i]);
		}
		g_gfxStandFront=bb_gfx_GFX.g_Tileset.m_GrabImage(0,80,16,16,1,1);
	}
	internal static var g_NextDog:int;
	public override function m_Activate():void{
		super.m_Activate();
	}
	public static function g_Create(t_tX:Number,t_tY:Number):int{
		var t_tDog:int=g_NextDog;
		g_a[g_NextDog].m_Activate();
		g_a[g_NextDog].m_SetPosition(t_tX,t_tY);
		g_NextDog+=1;
		if(g_NextDog==10){
			g_NextDog=0;
		}
		return t_tDog;
	}
	public override function m_Update():void{
		if(!this.m_IsOnScreen(0)){
			this.m_Deactivate();
		}
	}
	public static function g_UpdateAll():void{
		for(var t_i:int=0;t_i<10;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override function m_Render():void{
		bb_gfx_GFX.g_Draw(g_gfxStandFront,this.f_X,this.f_Y,0,true);
	}
	public static function g_RenderAll():void{
		for(var t_i:int=0;t_i<10;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_yeti_Yeti extends bb_entity_Entity{
	internal static var g_a:Array;
	public function g_Yeti_new(t_tLev:bb_level_Level):bb_yeti_Yeti{
		super.g_Entity_new2();
		this.f_level=t_tLev;
		this.f_EnType=8;
		this.f_W=20.0;
		this.f_H=30.0;
		return this;
	}
	public function g_Yeti_new2():bb_yeti_Yeti{
		super.g_Entity_new2();
		return this;
	}
	internal static var g_gfxStandFront:bb_graphics_Image;
	internal static var g_gfxRunFront:bb_graphics_Image;
	internal static var g_gfxRunLeft:bb_graphics_Image;
	internal static var g_gfxRunRight:bb_graphics_Image;
	internal static var g_gfxShadow:bb_graphics_Image;
	public static function g_Init(t_tLev:bb_level_Level):void{
		g_a=new_object_array(1);
		bb_entity_Entity.g_a[8]=new_object_array(1);
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			g_a[t_i]=(new bb_yeti_Yeti).g_Yeti_new(t_tLev);
			bb_entity_Entity.g_a[8][t_i]=(g_a[t_i]);
		}
		g_gfxStandFront=bb_gfx_GFX.g_Tileset.m_GrabImage(48,0,22,32,2,1);
		g_gfxRunFront=bb_gfx_GFX.g_Tileset.m_GrabImage(48,32,22,32,2,1);
		g_gfxRunLeft=bb_gfx_GFX.g_Tileset.m_GrabImage(48,64,22,32,2,1);
		g_gfxRunRight=bb_gfx_GFX.g_Tileset.m_GrabImage(48,96,22,32,2,1);
		g_gfxShadow=bb_gfx_GFX.g_Tileset.m_GrabImage(112,0,16,6,1,1);
	}
	internal static var g_NextYeti:int;
	internal var f_Status:int=0;
	public override function m_Activate():void{
		super.m_Activate();
		this.f_Status=0;
	}
	public static function g_Create(t_tX:Number,t_tY:Number):int{
		var t_thisYeti:int=g_NextYeti;
		g_a[t_thisYeti].m_Activate();
		g_NextYeti+=1;
		if(g_NextYeti==1){
			g_NextYeti=0;
		}
		return t_thisYeti;
	}
	public function m_StartWaiting():void{
		this.f_Status=0;
	}
	internal var f_aniRunFrameTimer:Number=0.0;
	internal var f_aniRunFrame:int=0;
	internal var f_aniWaitFrameTimer:Number=0.0;
	internal var f_aniWaitFrame:int=0;
	internal var f_D:int=0;
	public function m_UpdateControlled():void{
		if(bb_controls_Controls.g_LeftHit){
			if(this.f_D>0){
				this.f_D-=1;
			}else{
				if(this.f_onFloor){
					this.f_XS=-1.0;
				}
			}
		}
		if(bb_controls_Controls.g_RightHit){
			if(this.f_D<6){
				this.f_D+=1;
			}else{
				if(this.f_onFloor){
					this.f_XS=1.0;
				}
			}
		}
		if(bb_controls_Controls.g_DownHit){
			this.f_D=3;
		}
		if(bb_controls_Controls.g_UpHit){
			if(this.f_onFloor){
				this.f_ZS=-1.5;
				this.f_XS*=1.5;
				this.f_YS*=1.5;
			}
		}
	}
	internal var f_MaxYS:Number=.0;
	internal var f_TargetXS:Number=.0;
	public function m_UpdateChasing():void{
		this.m_UpdateControlled();
		if(this.f_onFloor){
			var t_:int=this.f_D;
			if(t_==0){
				this.f_MaxYS=0.0;
				this.f_TargetXS=0.0;
			}else{
				if(t_==1){
					this.f_MaxYS=1.0;
					this.f_TargetXS=-2.0;
				}else{
					if(t_==2){
						this.f_MaxYS=2.0;
						this.f_TargetXS=-1.0;
					}else{
						if(t_==3){
							this.f_MaxYS=3.0;
							this.f_TargetXS=0.0;
						}else{
							if(t_==4){
								this.f_MaxYS=2.0;
								this.f_TargetXS=1.0;
							}else{
								if(t_==5){
									this.f_MaxYS=1.0;
									this.f_TargetXS=2.0;
								}else{
									if(t_==6){
										this.f_MaxYS=0.0;
										this.f_TargetXS=0.0;
									}
								}
							}
						}
					}
				}
			}
			if(this.f_YS>this.f_MaxYS-0.05 && this.f_YS<this.f_MaxYS+0.05){
				this.f_YS=this.f_MaxYS;
			}else{
				if(this.f_YS>this.f_MaxYS){
					var t_tR:Number=0.02;
					if(this.f_D==0 || this.f_D==6){
						t_tR=0.04;
					}
					this.f_YS*=1.0-t_tR*bb_ld_LDApp.g_Delta;
				}else{
					this.f_YS+=0.05*bb_ld_LDApp.g_Delta;
				}
			}
			if(this.f_XS>this.f_TargetXS-0.05*bb_ld_LDApp.g_Delta && this.f_XS<this.f_TargetXS+0.05*bb_ld_LDApp.g_Delta){
				this.f_XS=this.f_TargetXS;
			}else{
				if(this.f_XS<this.f_TargetXS){
					this.f_XS+=0.05*bb_ld_LDApp.g_Delta;
				}else{
					if(this.f_XS>this.f_TargetXS){
						this.f_XS-=0.05*bb_ld_LDApp.g_Delta;
					}
				}
			}
		}else{
			this.f_ZS+=0.05*bb_ld_LDApp.g_Delta;
		}
		this.f_X+=this.f_XS*bb_ld_LDApp.g_Delta;
		this.f_Y+=this.f_YS*bb_ld_LDApp.g_Delta;
		this.f_Z+=this.f_ZS*bb_ld_LDApp.g_Delta;
	}
	public function m_UpdateDazed():void{
	}
	public function m_UpdateEating():void{
	}
	public function m_UpdateWaitingHappy():void{
	}
	public function m_StartChasing():void{
		this.f_D=3;
		this.f_XS=0.0;
		this.f_YS=0.5;
		this.f_ZS=-1.0;
		this.f_Z=0.0;
		this.f_Status=1;
	}
	public function m_UpdateWaiting():void{
		if(bb_controls_Controls.g_DownHit){
			this.m_StartChasing();
		}
	}
	public override function m_Update():void{
		super.m_Update();
		this.f_aniRunFrameTimer+=1.0*bb_ld_LDApp.g_Delta;
		if(this.f_aniRunFrameTimer>=5.0){
			this.f_aniRunFrameTimer=0.0;
			this.f_aniRunFrame+=1;
			if(this.f_aniRunFrame>=2){
				this.f_aniRunFrame=0;
			}
		}
		this.f_aniWaitFrameTimer+=1.0*bb_ld_LDApp.g_Delta;
		if(this.f_aniWaitFrameTimer>=30.0){
			this.f_aniWaitFrameTimer=0.0;
			this.f_aniWaitFrame+=1;
			if(this.f_aniWaitFrame>=2){
				this.f_aniWaitFrame=0;
			}
		}
		var t_:int=this.f_Status;
		if(t_==1){
			this.m_UpdateChasing();
		}else{
			if(t_==3){
				this.m_UpdateDazed();
			}else{
				if(t_==2){
					this.m_UpdateEating();
				}else{
					if(t_==4){
						this.m_UpdateWaitingHappy();
					}else{
						if(t_==0){
							this.m_UpdateWaiting();
						}
					}
				}
			}
		}
		if(!this.m_IsOnScreen(0)){
			this.m_Deactivate();
		}
	}
	public static function g_UpdateAll():void{
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public function m_RenderChasing():void{
		var t_:int=this.f_D;
		if(t_==0){
			bb_gfx_GFX.g_Draw(g_gfxStandFront,this.f_X,this.f_Y+this.f_Z,this.f_aniWaitFrame,true);
		}else{
			if(t_==1 || t_==2){
				bb_gfx_GFX.g_Draw(g_gfxRunLeft,this.f_X,this.f_Y+this.f_Z,this.f_aniRunFrame,true);
			}else{
				if(t_==3){
					bb_gfx_GFX.g_Draw(g_gfxRunFront,this.f_X,this.f_Y+this.f_Z,this.f_aniRunFrame,true);
				}else{
					if(t_==4 || t_==5){
						bb_gfx_GFX.g_Draw(g_gfxRunRight,this.f_X,this.f_Y+this.f_Z,this.f_aniRunFrame,true);
					}else{
						if(t_==6){
							bb_gfx_GFX.g_Draw(g_gfxStandFront,this.f_X,this.f_Y+this.f_Z,this.f_aniWaitFrame,true);
						}
					}
				}
			}
		}
	}
	public function m_RenderDazed():void{
	}
	public function m_RenderEating():void{
	}
	public function m_RenderWaitingHappy():void{
	}
	public function m_RenderWaiting():void{
		bb_gfx_GFX.g_Draw(g_gfxStandFront,this.f_X,this.f_Y,this.f_aniWaitFrame,true);
	}
	public override function m_Render():void{
		bb_graphics_SetAlpha(0.25);
		bb_gfx_GFX.g_Draw(g_gfxShadow,this.f_X,this.f_Y+12.0,0,true);
		bb_graphics_SetAlpha(1.0);
		var t_:int=this.f_Status;
		if(t_==1){
			this.m_RenderChasing();
		}else{
			if(t_==3){
				this.m_RenderDazed();
			}else{
				if(t_==2){
					this.m_RenderEating();
				}else{
					if(t_==4){
						this.m_RenderWaitingHappy();
					}else{
						if(t_==0){
							this.m_RenderWaiting();
						}else{
							if(t_==5){
							}
						}
					}
				}
			}
		}
	}
	public static function g_RenderAll():void{
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_skier_Skier extends bb_entity_Entity{
	internal static var g_a:Array;
	internal var f_TargetYeti:int=0;
	public function g_Skier_new(t_tLev:bb_level_Level):bb_skier_Skier{
		super.g_Entity_new2();
		this.f_level=t_tLev;
		this.f_EnType=5;
		this.f_W=12.0;
		this.f_H=16.0;
		this.f_TargetYeti=0;
		return this;
	}
	public function g_Skier_new2():bb_skier_Skier{
		super.g_Entity_new2();
		return this;
	}
	internal static var g_gfxSki:bb_graphics_Image;
	internal static var g_gfxTease:bb_graphics_Image;
	internal static var g_gfxShadow:bb_graphics_Image;
	internal static var g_gfxFall:bb_graphics_Image;
	public static function g_Init(t_tLev:bb_level_Level):void{
		g_a=new_object_array(1);
		bb_entity_Entity.g_a[5]=new_object_array(1);
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			g_a[t_i]=(new bb_skier_Skier).g_Skier_new(t_tLev);
			bb_entity_Entity.g_a[5][t_i]=(g_a[t_i]);
		}
		g_gfxSki=bb_gfx_GFX.g_Tileset.m_GrabImage(0,128,16,32,7,1);
		g_gfxTease=bb_gfx_GFX.g_Tileset.m_GrabImage(112,128,16,32,2,1);
		g_gfxShadow=bb_gfx_GFX.g_Tileset.m_GrabImage(0,160,16,4,1,1);
		g_gfxFall=bb_gfx_GFX.g_Tileset.m_GrabImage(144,128,16,32,4,1);
	}
	internal static var g_NextSkier:int;
	public override function m_Activate():void{
		super.m_Activate();
		this.f_TargetYeti=-1;
	}
	public static function g_Create(t_tX:Number,t_tY:Number):int{
		var t_tI:int=g_NextSkier;
		g_a[t_tI].m_Activate();
		g_a[t_tI].m_SetPosition(t_tX,t_tY);
		g_NextSkier+=1;
		if(g_NextSkier>=1){
			g_NextSkier=0;
		}
		return t_tI;
	}
	internal var f_Status:int=0;
	internal var f_D:int=0;
	public function m_StartPreTeasing():void{
		this.f_Status=5;
		this.f_D=3;
		this.f_YS=1.6;
	}
	public function m_UpdateDazed():void{
	}
	public function m_UpdateDead():void{
	}
	internal var f_fallTimer:Number=.0;
	internal var f_fallFrameTimer:Number=.0;
	internal var f_fallFrame:int=0;
	public function m_ContinueSkiing():void{
		this.f_Status=0;
		this.f_Z=0.0;
		this.f_D=3;
		this.f_ZS=0.0;
	}
	public function m_UpdateFalling():void{
		this.f_fallTimer+=1.0*bb_ld_LDApp.g_Delta;
		this.f_fallFrameTimer+=1.0*bb_ld_LDApp.g_Delta;
		if(this.f_fallFrameTimer>=7.0){
			this.f_fallFrameTimer=0.0;
			this.f_fallFrame+=1;
			if(this.f_fallFrame==4){
				this.f_fallFrame=0;
			}
		}
		if(this.f_onFloor==true){
			if(this.f_ZS>=4.0){
				this.f_ZS=0.0-this.f_ZS*0.5;
				this.f_XS=this.f_XS*0.6;
				this.f_YS=this.f_YS*0.6;
			}else{
				if(this.f_fallTimer>=15.0){
					this.m_ContinueSkiing();
				}else{
					print(String(this.f_fallTimer));
				}
			}
		}
		this.f_X+=this.f_XS*bb_ld_LDApp.g_Delta;
		this.f_Y+=this.f_YS*bb_ld_LDApp.g_Delta;
		this.f_Z+=this.f_ZS*bb_ld_LDApp.g_Delta;
		this.f_ZS+=0.05*bb_ld_LDApp.g_Delta;
	}
	internal var f_MaxYS:Number=.0;
	internal var f_TargetXS:Number=.0;
	public function m_StartFalling():void{
		var t_tZ:Number=(bb_math_Abs2(this.f_XS)+bb_math_Abs2(this.f_YS))*0.5;
		this.f_ZS=0.0-t_tZ;
		this.f_Status=1;
		this.f_fallTimer=0.0;
	}
	public function m_UpdateSkiing():void{
		if(bb_random_Rnd()<0.02){
			if(bb_random_Rnd()<0.5){
				this.f_D-=1;
			}else{
				this.f_D+=1;
			}
		}
		this.f_D=bb_math_Clamp(this.f_D,1,5);
		if(this.f_onFloor){
			var t_:int=this.f_D;
			if(t_==0){
				this.f_MaxYS=0.0;
				this.f_TargetXS=0.0;
			}else{
				if(t_==1){
					this.f_MaxYS=1.0;
					this.f_TargetXS=-2.0;
				}else{
					if(t_==2){
						this.f_MaxYS=2.0;
						this.f_TargetXS=-1.0;
					}else{
						if(t_==3){
							this.f_MaxYS=3.2;
							this.f_TargetXS=0.0;
						}else{
							if(t_==4){
								this.f_MaxYS=2.0;
								this.f_TargetXS=1.0;
							}else{
								if(t_==5){
									this.f_MaxYS=1.0;
									this.f_TargetXS=2.0;
								}else{
									if(t_==6){
										this.f_MaxYS=0.0;
										this.f_TargetXS=0.0;
									}
								}
							}
						}
					}
				}
			}
			if(this.f_YS>this.f_MaxYS-0.05 && this.f_YS<this.f_MaxYS+0.05){
				this.f_YS=this.f_MaxYS;
			}else{
				if(this.f_YS>this.f_MaxYS){
					this.f_YS*=1.0-0.02*bb_ld_LDApp.g_Delta;
				}else{
					if(this.f_YS<this.f_MaxYS){
						this.f_YS+=0.05*bb_ld_LDApp.g_Delta;
					}
				}
			}
			if(this.f_XS>this.f_TargetXS-0.05*bb_ld_LDApp.g_Delta && this.f_XS<this.f_TargetXS+0.05*bb_ld_LDApp.g_Delta){
				this.f_XS=this.f_TargetXS;
			}else{
				if(this.f_XS<this.f_TargetXS){
					this.f_XS+=0.05*bb_ld_LDApp.g_Delta;
				}else{
					if(this.f_XS>this.f_TargetXS){
						this.f_XS-=0.05*bb_ld_LDApp.g_Delta;
					}
				}
			}
		}else{
			this.f_ZS+=0.05*bb_ld_LDApp.g_Delta;
		}
		this.f_X+=this.f_XS*bb_ld_LDApp.g_Delta;
		this.f_Y+=this.f_YS*bb_ld_LDApp.g_Delta;
		this.f_Z+=this.f_ZS*bb_ld_LDApp.g_Delta;
		var t_colStatus:int=this.m_CheckForCollision();
		var t_2:int=t_colStatus;
		if(t_2==0){
			this.f_ZS=-1.0;
		}else{
			if(t_2==1){
				this.f_ZS=-1.0;
				this.f_XS*=0.9;
				this.f_YS*=0.9;
			}else{
				if(t_2==2){
					this.f_XS*=0.9;
					this.f_YS*=0.9;
				}else{
					if(t_2==3){
						this.f_XS=this.f_XS*2.0;
						this.f_YS=this.f_YS*2.0;
						this.f_ZS=-3.0;
					}else{
						if(t_2==4){
							this.m_StartFalling();
							this.f_XS*=0.5;
							this.f_YS*=0.5;
						}else{
							if(t_2==6){
							}else{
								if(t_2==7){
									this.m_StartFalling();
									this.f_XS*=0.7;
									this.f_YS*=0.7;
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
	internal var f_teasingFrameTimer:Number=.0;
	internal var f_teasingFrame:int=0;
	public function m_StartSkiing():void{
		this.f_Status=0;
		this.f_Z=0.0;
		this.f_D=3;
		this.f_YS=4.0;
		this.f_XS=0.0;
		this.f_ZS=-2.0;
		bb_sfx_SFX.g_Music("chase",1);
	}
	public function m_UpdateTeasing():void{
		if(this.f_YS==0.0){
			if(this.f_teasingFrameTimer>0.0){
				this.f_teasingFrameTimer-=1.0*bb_ld_LDApp.g_Delta;
				this.f_teasingFrame=1;
			}else{
				this.f_teasingFrameTimer=0.0;
				this.f_teasingFrame=0;
			}
			if(bb_random_Rnd()<0.04){
				this.f_Y=this.f_Y+1.0;
				this.f_teasingFrameTimer=5.0;
			}
		}else{
			this.f_Y+=this.f_YS*bb_ld_LDApp.g_Delta;
		}
		if(this.f_YS<=0.1){
			this.f_YS=0.0;
		}else{
			this.f_YS*=1.0-0.05*bb_ld_LDApp.g_Delta;
		}
		if(bb_yeti_Yeti.g_a[this.f_TargetYeti].f_Y-this.f_Y<2.0){
			this.m_StartSkiing();
		}else{
			if(bb_yeti_Yeti.g_a[this.f_TargetYeti].f_Y-this.f_Y<50.0){
				if(bb_random_Rnd()<0.02){
					this.m_StartSkiing();
				}
			}
		}
	}
	public function m_FindNearestYeti():int{
		var t_nIndex:int=-1;
		var t_nDistance:Number=99999999.0;
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			if(bb_yeti_Yeti.g_a[t_i].f_Active==true){
				if(bb_yeti_Yeti.g_a[t_i].f_Y>this.f_Y){
					var t_tDist:Number=bb_yeti_Yeti.g_a[t_i].f_Y-this.f_Y;
					if(t_tDist<200.0){
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
	public function m_StartTeasing():void{
		this.f_Status=4;
		this.f_D=3;
		this.f_TargetYeti=this.m_FindNearestYeti();
	}
	public function m_UpdatePreTeasing():void{
		this.f_Y+=this.f_YS*bb_ld_LDApp.g_Delta;
		if(this.f_TargetYeti>=0){
			if(bb_yeti_Yeti.g_a[this.f_TargetYeti].f_Y-this.f_Y<130.0){
				if(bb_random_Rnd()<0.02){
					this.f_D=0;
					this.m_StartTeasing();
				}
			}
		}
	}
	public override function m_Update():void{
		super.m_Update();
		var t_:int=this.f_Status;
		if(t_==2){
			this.m_UpdateDazed();
		}else{
			if(t_==3){
				this.m_UpdateDead();
			}else{
				if(t_==1){
					this.m_UpdateFalling();
				}else{
					if(t_==0){
						this.m_UpdateSkiing();
					}else{
						if(t_==4){
							this.m_UpdateTeasing();
						}else{
							if(t_==5){
								this.m_UpdatePreTeasing();
							}
						}
					}
				}
			}
		}
	}
	public static function g_UpdateAll():void{
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override function m_Render():void{
		if(this.f_Z<0.0){
			bb_graphics_SetAlpha(0.25);
			bb_gfx_GFX.g_Draw(g_gfxShadow,this.f_X-this.f_Z*1.0,this.f_Y+7.0-this.f_Z*2.0,0,true);
		}
		bb_graphics_SetAlpha(1.0);
		var t_:int=this.f_Status;
		if(t_==4){
			bb_gfx_GFX.g_Draw(g_gfxTease,this.f_X,this.f_Y+this.f_Z,this.f_teasingFrame,true);
		}else{
			if(t_==5){
				bb_gfx_GFX.g_Draw(g_gfxSki,this.f_X,this.f_Y+this.f_Z,this.f_D,true);
			}else{
				if(t_==1){
					bb_gfx_GFX.g_Draw(g_gfxFall,this.f_X,this.f_Y+this.f_Z,this.f_fallFrame,true);
				}else{
					bb_gfx_GFX.g_Draw(g_gfxSki,this.f_X,this.f_Y+this.f_Z,this.f_D,true);
				}
			}
		}
		bb_graphics_DrawText(String(this.f_YS),10.0,10.0,0.0,0.0);
		bb_graphics_DrawText(String(this.f_TargetYeti),10.0,30.0,0.0,0.0);
	}
	public function m_RenderMarker():void{
		var t_dX:int=((this.f_X-(bb_ld_LDApp.g_ScreenX))|0);
		var t_dY:int=((this.f_Y-(bb_ld_LDApp.g_ScreenY))|0);
		var t_hState:int=0;
		var t_vState:int=0;
		if(t_dX<0){
			t_hState=-1;
		}else{
			if(t_dX>bb_ld_LDApp.g_ScreenWidth){
				t_hState=1;
			}
		}
		if(t_dY<0){
			t_vState=-1;
		}else{
			if(t_dY>bb_ld_LDApp.g_ScreenHeight){
				t_vState=1;
			}
		}
		var t_:int=t_hState;
		if(t_==-1){
			var t_2:int=t_vState;
			if(t_2==-1){
				bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0,1.0,16,192,8,8,0);
			}else{
				if(t_2==0){
					bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0,(t_dY-4),0,176,8,9,0);
				}else{
					if(t_2==1){
						bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,1.0,(bb_ld_LDApp.g_ScreenHeight-9),0,192,8,8,0);
					}
				}
			}
		}else{
			if(t_==0){
				var t_3:int=t_vState;
				if(t_3==-1){
					bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(t_dX-4),1.0,32,176,9,8,0);
				}else{
					if(t_3==0){
					}else{
						if(t_3==1){
							bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(t_dX-4),(bb_ld_LDApp.g_ScreenHeight-10),16,176,9,8,0);
						}
					}
				}
			}else{
				if(t_==1){
					var t_4:int=t_vState;
					if(t_4==-1){
						bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(bb_ld_LDApp.g_ScreenWidth-9),1.0,24,192,8,8,0);
					}else{
						if(t_4==0){
							bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(bb_ld_LDApp.g_ScreenWidth-9),(t_dY-4),8,176,8,9,0);
						}else{
							if(t_4==1){
								bb_graphics_DrawImageRect(bb_gfx_GFX.g_Tileset,(bb_ld_LDApp.g_ScreenWidth-9),(bb_ld_LDApp.g_ScreenHeight-9),8,192,8,8,0);
							}
						}
					}
				}
			}
		}
	}
	public static function g_RenderAll():void{
		for(var t_i:int=0;t_i<1;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Render();
				g_a[t_i].m_RenderMarker();
			}
		}
	}
}
class bb_tree_Tree extends bb_entity_Entity{
	internal static var g_a:Array;
	public function g_Tree_new(t_tLev:bb_level_Level):bb_tree_Tree{
		super.g_Entity_new2();
		this.f_level=t_tLev;
		this.f_EnType=7;
		this.f_W=10.0;
		this.f_H=10.0;
		return this;
	}
	public function g_Tree_new2():bb_tree_Tree{
		super.g_Entity_new2();
		return this;
	}
	internal static var g_gfxSmallTree:bb_graphics_Image;
	internal static var g_gfxBigTree:bb_graphics_Image;
	internal static var g_gfxTreeStump:bb_graphics_Image;
	public static function g_Init(t_tLev:bb_level_Level):void{
		g_a=new_object_array(1000);
		bb_entity_Entity.g_a[7]=new_object_array(1000);
		for(var t_i:int=0;t_i<1000;t_i=t_i+1){
			g_a[t_i]=(new bb_tree_Tree).g_Tree_new(t_tLev);
			bb_entity_Entity.g_a[7][t_i]=(g_a[t_i]);
		}
		g_gfxSmallTree=bb_gfx_GFX.g_Tileset.m_GrabImage(32,256,16,16,1,1);
		g_gfxBigTree=bb_gfx_GFX.g_Tileset.m_GrabImage(0,240,16,32,1,1);
		g_gfxTreeStump=bb_gfx_GFX.g_Tileset.m_GrabImage(16,256,16,16,1,1);
	}
	internal static var g_NextTree:int;
	public override function m_Activate():void{
		super.m_Activate();
	}
	internal var f_Type:int=0;
	public static function g_Create(t_tX:Number,t_tY:Number):int{
		var t_tT:int=g_NextTree;
		g_a[g_NextTree].m_Activate();
		g_a[g_NextTree].m_SetPosition(t_tX,t_tY);
		var t_chance:Number=bb_random_Rnd();
		var t_tType:int=0;
		if(t_chance<0.5){
			t_tType=0;
		}else{
			if(t_chance<0.8){
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
	public override function m_Update():void{
	}
	public static function g_UpdateAll():void{
		for(var t_i:int=0;t_i<1000;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override function m_Render():void{
		var t_:int=this.f_Type;
		if(t_==0){
			bb_gfx_GFX.g_Draw(g_gfxBigTree,this.f_X,this.f_Y,0,true);
		}else{
			if(t_==1){
				bb_gfx_GFX.g_Draw(g_gfxSmallTree,this.f_X,this.f_Y,0,true);
			}else{
				if(t_==2){
					bb_gfx_GFX.g_Draw(g_gfxTreeStump,this.f_X,this.f_Y,0,true);
				}
			}
		}
	}
	public static function g_RenderAll():void{
		for(var t_i:int=0;t_i<1000;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_rock_Rock extends bb_entity_Entity{
	internal static var g_a:Array;
	public function g_Rock_new(t_tLev:bb_level_Level):bb_rock_Rock{
		super.g_Entity_new2();
		this.f_level=t_tLev;
		this.f_EnType=4;
		this.f_W=10.0;
		this.f_H=10.0;
		return this;
	}
	public function g_Rock_new2():bb_rock_Rock{
		super.g_Entity_new2();
		return this;
	}
	internal static var g_gfxRockBoulder:bb_graphics_Image;
	internal static var g_gfxRockSpikey:bb_graphics_Image;
	internal static var g_gfxRockFlat:bb_graphics_Image;
	public static function g_Init(t_tLev:bb_level_Level):void{
		g_a=new_object_array(300);
		bb_entity_Entity.g_a[4]=new_object_array(300);
		for(var t_i:int=0;t_i<300;t_i=t_i+1){
			g_a[t_i]=(new bb_rock_Rock).g_Rock_new(t_tLev);
			bb_entity_Entity.g_a[4][t_i]=(g_a[t_i]);
		}
		g_gfxRockBoulder=bb_gfx_GFX.g_Tileset.m_GrabImage(0,288,16,16,1,1);
		g_gfxRockSpikey=bb_gfx_GFX.g_Tileset.m_GrabImage(16,288,16,16,1,1);
		g_gfxRockFlat=bb_gfx_GFX.g_Tileset.m_GrabImage(32,288,16,16,1,1);
	}
	internal static var g_NextRock:int;
	internal var f_Type:int=0;
	public override function m_Activate():void{
		super.m_Activate();
	}
	public static function g_Create(t_tX:Number,t_tY:Number):int{
		var t_tRock:int=g_NextRock;
		var t_chance:Number=bb_random_Rnd();
		var t_tType:int=0;
		if(t_chance<0.33){
			t_tType=0;
		}else{
			if(t_chance<0.66){
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
	public override function m_Update():void{
	}
	public static function g_UpdateAll():void{
		for(var t_i:int=0;t_i<300;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				g_a[t_i].m_Update();
			}
		}
	}
	public override function m_Render():void{
		var t_:int=this.f_Type;
		if(t_==0){
			bb_gfx_GFX.g_Draw(g_gfxRockBoulder,this.f_X,this.f_Y,0,true);
		}else{
			if(t_==2){
				bb_gfx_GFX.g_Draw(g_gfxRockFlat,this.f_X,this.f_Y,0,true);
			}else{
				if(t_==1){
					bb_gfx_GFX.g_Draw(g_gfxRockSpikey,this.f_X,this.f_Y,0,true);
				}
			}
		}
	}
	public static function g_RenderAll():void{
		for(var t_i:int=0;t_i<300;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
}
class bb_flag_Flag extends bb_entity_Entity{
	internal static var g_a:Array;
	public function g_Flag_new(t_tLev:bb_level_Level):bb_flag_Flag{
		super.g_Entity_new2();
		this.f_level=t_tLev;
		this.f_EnType=2;
		this.f_W=8.0;
		this.f_H=16.0;
		return this;
	}
	public function g_Flag_new2():bb_flag_Flag{
		super.g_Entity_new2();
		return this;
	}
	internal static var g_gfxFlag:bb_graphics_Image;
	internal static var g_NextFlag:int;
	public static function g_Init(t_tLev:bb_level_Level):void{
		g_a=new_object_array(150);
		bb_entity_Entity.g_a[2]=new_object_array(150);
		for(var t_i:int=0;t_i<150;t_i=t_i+1){
			g_a[t_i]=(new bb_flag_Flag).g_Flag_new(t_tLev);
			bb_entity_Entity.g_a[2][t_i]=(g_a[t_i]);
		}
		g_gfxFlag=bb_gfx_GFX.g_Tileset.m_GrabImage(0,272,16,16,2,1);
		g_NextFlag=0;
	}
	public override function m_Activate():void{
		super.m_Activate();
	}
	public static function g_Create(t_tX:Number,t_tY:Number):int{
		var t_tFlag:int=g_NextFlag;
		g_a[g_NextFlag].m_Activate();
		g_a[g_NextFlag].m_SetPosition(t_tX,t_tY);
		g_NextFlag+=1;
		if(g_NextFlag==150){
			g_NextFlag=0;
		}
		return t_tFlag;
	}
	internal var f_Type:int=0;
	public override function m_Render():void{
		bb_gfx_GFX.g_Draw(g_gfxFlag,this.f_X,this.f_Y,this.f_Type,true);
	}
	public static function g_RenderAll():void{
		for(var t_i:int=0;t_i<150;t_i=t_i+1){
			if(g_a[t_i].f_Active==true){
				if(g_a[t_i].m_IsOnScreen(0)){
					g_a[t_i].m_Render();
				}
			}
		}
	}
	public override function m_Update():void{
	}
}
class bb_yeti_YetiStatusTypes extends Object{
}
class bb_skier_SkierStatusType extends Object{
}
class bb_entity_EntityMoveDirection extends Object{
}
class bb_raztext_RazChar extends Object{
	public function g_RazChar_new():bb_raztext_RazChar{
		return this;
	}
	internal var f_XOff:int=0;
	internal var f_YOff:int=0;
	internal var f_TextValue:String="";
	internal var f_Visible:Boolean=true;
}
class bb_list_List extends Object{
	public function g_List_new():bb_list_List{
		return this;
	}
	internal var f__head:bb_list_Node=((new bb_list_HeadNode).g_HeadNode_new());
	public function m_AddLast(t_data:Array):bb_list_Node{
		return (new bb_list_Node).g_Node_new(this.f__head,this.f__head.f__pred,t_data);
	}
	public function g_List_new2(t_data:Array):bb_list_List{
		var t_:Array=t_data;
		var t_2:int=0;
		while(t_2<t_.length){
			var t_t:Array=t_[t_2];
			t_2=t_2+1;
			this.m_AddLast(t_t);
		}
		return this;
	}
}
class bb_list_Node extends Object{
	internal var f__succ:bb_list_Node=null;
	internal var f__pred:bb_list_Node=null;
	internal var f__data:Array=[];
	public function g_Node_new(t_succ:bb_list_Node,t_pred:bb_list_Node,t_data:Array):bb_list_Node{
		this.f__succ=t_succ;
		this.f__pred=t_pred;
		this.f__succ.f__pred=this;
		this.f__pred.f__succ=this;
		this.f__data=t_data;
		return this;
	}
	public function g_Node_new2():bb_list_Node{
		return this;
	}
}
class bb_list_HeadNode extends bb_list_Node{
	public function g_HeadNode_new():bb_list_HeadNode{
		super.g_Node_new2();
		this.f__succ=(this);
		this.f__pred=(this);
		return this;
	}
}
internal function bb_app_LoadString(t_path:String):String{
	return bb_app_device.LoadString(bb_data_FixDataPath(t_path));
}
class bb_flag_FlagType extends Object{
}
internal function bb_level_GenerateFlagTrail(t_tL:bb_level_Level):void{
	var t_tX:int=0;
	var t_subX:int=0;
	var t_tY:int=200;
	var t_tStep:int=200;
	var t_tLeft:Boolean=false;
	var t_tWidth:int=50;
	for(var t_i:int=0;t_i<150;t_i=t_i+1){
		t_tX=((Math.sin((t_tY)*D2R)*(t_tWidth))|0);
		t_subX=((Math.sin((t_tY*3)*D2R)*(t_tWidth)*0.1)|0);
		var t_tF:int=bb_flag_Flag.g_Create((t_tX+t_subX),(t_tY));
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
var bb_random_Seed:int;
internal function bb_random_Rnd():Number{
	bb_random_Seed=bb_random_Seed*1664525+1013904223|0;
	return (bb_random_Seed>>8&16777215)/16777216.0;
}
internal function bb_random_Rnd2(t_low:Number,t_high:Number):Number{
	return bb_random_Rnd3(t_high-t_low)+t_low;
}
internal function bb_random_Rnd3(t_range:Number):Number{
	return bb_random_Rnd()*t_range;
}
class bb_tree_TreeType extends Object{
}
internal function bb_level_GenerateTrees(t_tL:bb_level_Level):void{
	for(var t_i:int=0;t_i<1000;t_i=t_i+1){
		var t_tX:int=((bb_random_Rnd2(0.0-(bb_level_Level.g_Width)*0.5,(bb_level_Level.g_Width)*0.5))|0);
		var t_tY:int=((bb_random_Rnd2(0.0,(bb_level_Level.g_Height)))|0);
		bb_tree_Tree.g_Create((t_tX),(t_tY));
	}
}
class bb_rock_RockType extends Object{
}
internal function bb_level_GenerateRocks(t_tL:bb_level_Level):void{
	for(var t_i:int=0;t_i<300;t_i=t_i+1){
		var t_tGood:Boolean=false;
		var t_tX:int=0;
		var t_tY:int=0;
		while(t_tGood==false){
			t_tX=((bb_random_Rnd2(0.0-(bb_level_Level.g_Width)*0.5,(bb_level_Level.g_Width)*0.5))|0);
			t_tY=((bb_random_Rnd2(0.0,(bb_level_Level.g_Height)))|0);
			if(t_tY % 4000>2000){
				t_tGood=true;
			}
		}
		bb_rock_Rock.g_Create((t_tX),(t_tY));
	}
}
internal function bb_level_GenerateLevel():bb_level_Level{
	var t_tL:bb_level_Level=(new bb_level_Level).g_Level_new();
	bb_level_GenerateFlagTrail(t_tL);
	bb_level_GenerateTrees(t_tL);
	bb_level_GenerateRocks(t_tL);
	return t_tL;
}
internal function bb_audio_MusicState():int{
	return bb_audio_device.MusicState();
}
internal function bb_audio_PlayMusic(t_path:String,t_flags:int):int{
	return bb_audio_device.PlayMusic(bb_data_FixDataPath(t_path),t_flags);
}
internal function bb_functions_RectOverRect(t_tX1:Number,t_tY1:Number,t_tW1:Number,t_tH1:Number,t_tX2:Number,t_tY2:Number,t_tW2:Number,t_tH2:Number):Boolean{
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
internal function bb_math_Abs(t_x:int):int{
	if(t_x>=0){
		return t_x;
	}
	return -t_x;
}
internal function bb_math_Abs2(t_x:Number):Number{
	if(t_x>=0.0){
		return t_x;
	}
	return -t_x;
}
internal function bb_graphics_PushMatrix():int{
	var t_sp:int=bb_graphics_context.f_matrixSp;
	bb_graphics_context.f_matrixStack[t_sp+0]=bb_graphics_context.f_ix;
	bb_graphics_context.f_matrixStack[t_sp+1]=bb_graphics_context.f_iy;
	bb_graphics_context.f_matrixStack[t_sp+2]=bb_graphics_context.f_jx;
	bb_graphics_context.f_matrixStack[t_sp+3]=bb_graphics_context.f_jy;
	bb_graphics_context.f_matrixStack[t_sp+4]=bb_graphics_context.f_tx;
	bb_graphics_context.f_matrixStack[t_sp+5]=bb_graphics_context.f_ty;
	bb_graphics_context.f_matrixSp=t_sp+6;
	return 0;
}
internal function bb_graphics_PopMatrix():int{
	var t_sp:int=bb_graphics_context.f_matrixSp-6;
	bb_graphics_SetMatrix(bb_graphics_context.f_matrixStack[t_sp+0],bb_graphics_context.f_matrixStack[t_sp+1],bb_graphics_context.f_matrixStack[t_sp+2],bb_graphics_context.f_matrixStack[t_sp+3],bb_graphics_context.f_matrixStack[t_sp+4],bb_graphics_context.f_matrixStack[t_sp+5]);
	bb_graphics_context.f_matrixSp=t_sp;
	return 0;
}
internal function bb_graphics_DrawImage(t_image:bb_graphics_Image,t_x:Number,t_y:Number,t_frame:int):int{
	var t_f:bb_graphics_Frame=t_image.f_frames[t_frame];
	if((bb_graphics_context.f_tformed)!=0){
		bb_graphics_PushMatrix();
		bb_graphics_Translate(t_x-t_image.f_tx,t_y-t_image.f_ty);
		bb_graphics_context.m_Validate();
		if((t_image.f_flags&65536)!=0){
			bb_graphics_renderDevice.DrawSurface(t_image.f_surface,0.0,0.0);
		}else{
			bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0,0.0,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
		}
		bb_graphics_PopMatrix();
	}else{
		bb_graphics_context.m_Validate();
		if((t_image.f_flags&65536)!=0){
			bb_graphics_renderDevice.DrawSurface(t_image.f_surface,t_x-t_image.f_tx,t_y-t_image.f_ty);
		}else{
			bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,t_x-t_image.f_tx,t_y-t_image.f_ty,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
		}
	}
	return 0;
}
internal function bb_graphics_Rotate(t_angle:Number):int{
	bb_graphics_Transform(Math.cos((t_angle)*D2R),-Math.sin((t_angle)*D2R),Math.sin((t_angle)*D2R),Math.cos((t_angle)*D2R),0.0,0.0);
	return 0;
}
internal function bb_graphics_DrawImage2(t_image:bb_graphics_Image,t_x:Number,t_y:Number,t_rotation:Number,t_scaleX:Number,t_scaleY:Number,t_frame:int):int{
	var t_f:bb_graphics_Frame=t_image.f_frames[t_frame];
	bb_graphics_PushMatrix();
	bb_graphics_Translate(t_x,t_y);
	bb_graphics_Rotate(t_rotation);
	bb_graphics_Scale(t_scaleX,t_scaleY);
	bb_graphics_Translate(-t_image.f_tx,-t_image.f_ty);
	bb_graphics_context.m_Validate();
	if((t_image.f_flags&65536)!=0){
		bb_graphics_renderDevice.DrawSurface(t_image.f_surface,0.0,0.0);
	}else{
		bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0,0.0,t_f.f_x,t_f.f_y,t_image.f_width,t_image.f_height);
	}
	bb_graphics_PopMatrix();
	return 0;
}
internal function bb_graphics_DrawImageRect(t_image:bb_graphics_Image,t_x:Number,t_y:Number,t_srcX:int,t_srcY:int,t_srcWidth:int,t_srcHeight:int,t_frame:int):int{
	var t_f:bb_graphics_Frame=t_image.f_frames[t_frame];
	if((bb_graphics_context.f_tformed)!=0){
		bb_graphics_PushMatrix();
		bb_graphics_Translate(-t_image.f_tx+t_x,-t_image.f_ty+t_y);
		bb_graphics_context.m_Validate();
		bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0,0.0,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
		bb_graphics_PopMatrix();
	}else{
		bb_graphics_context.m_Validate();
		bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,-t_image.f_tx+t_x,-t_image.f_ty+t_y,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
	}
	return 0;
}
internal function bb_graphics_DrawImageRect2(t_image:bb_graphics_Image,t_x:Number,t_y:Number,t_srcX:int,t_srcY:int,t_srcWidth:int,t_srcHeight:int,t_rotation:Number,t_scaleX:Number,t_scaleY:Number,t_frame:int):int{
	var t_f:bb_graphics_Frame=t_image.f_frames[t_frame];
	bb_graphics_PushMatrix();
	bb_graphics_Translate(t_x,t_y);
	bb_graphics_Rotate(t_rotation);
	bb_graphics_Scale(t_scaleX,t_scaleY);
	bb_graphics_Translate(-t_image.f_tx,-t_image.f_ty);
	bb_graphics_context.m_Validate();
	bb_graphics_renderDevice.DrawSurface2(t_image.f_surface,0.0,0.0,t_srcX+t_f.f_x,t_srcY+t_f.f_y,t_srcWidth,t_srcHeight);
	bb_graphics_PopMatrix();
	return 0;
}
internal function bb_graphics_DrawText(t_text:String,t_x:Number,t_y:Number,t_xalign:Number,t_yalign:Number):int{
	if(!((bb_graphics_context.f_font)!=null)){
		return 0;
	}
	var t_w:int=bb_graphics_context.f_font.m_Width();
	var t_h:int=bb_graphics_context.f_font.m_Height();
	t_x-=Math.floor((t_w*t_text.length)*t_xalign);
	t_y-=Math.floor((t_h)*t_yalign);
	for(var t_i:int=0;t_i<t_text.length;t_i=t_i+1){
		var t_ch:int=t_text.charCodeAt(t_i)-bb_graphics_context.f_firstChar;
		if(t_ch>=0 && t_ch<bb_graphics_context.f_font.m_Frames()){
			bb_graphics_DrawImage(bb_graphics_context.f_font,t_x+(t_i*t_w),t_y,t_ch);
		}
	}
	return 0;
}
function bbInit():void{
	bb_graphics_device=null;
	bb_input_device=null;
	bb_audio_device=null;
	bb_app_device=null;
	bb_graphics_context=(new bb_graphics_GraphicsContext).g_GraphicsContext_new();
	bb_graphics_Image.g_DefaultFlags=0;
	bb_graphics_renderDevice=null;
	bb_gfx_GFX.g_Tileset=null;
	bb_gfx_GFX.g_Overlay=null;
	bb_gfx_GFX.g_Title=null;
	bb_screenmanager_ScreenManager.g_Screens=null;
	bb_screenmanager_ScreenManager.g_FadeAlpha=.0;
	bb_screenmanager_ScreenManager.g_FadeRate=.0;
	bb_screenmanager_ScreenManager.g_FadeRed=.0;
	bb_screenmanager_ScreenManager.g_FadeGreen=.0;
	bb_screenmanager_ScreenManager.g_FadeBlue=.0;
	bb_screenmanager_ScreenManager.g_FadeMode=0;
	bb_sfx_SFX.g_ActiveChannel=0;
	bb_sfx_SFX.g_Sounds=null;
	bb_sfx_SFX.g_Musics=null;
	bb_sfx_SFX.g_SoundFileAppendix=".wav";
	bb_controls_Controls.g_TCMove=null;
	bb_ld_LDApp.g_ScreenHeight=360;
	bb_ld_LDApp.g_ScreenWidth=240;
	bb_controls_Controls.g_TCAction1=null;
	bb_controls_Controls.g_TCAction2=null;
	bb_controls_Controls.g_TCEscapeKey=null;
	bb_controls_Controls.g_TCButtons=[];
	bb_raztext_RazText.g_TextSheet=null;
	bb_screenmanager_ScreenManager.g_gameScreen=null;
	bb_screenmanager_ScreenManager.g_ActiveScreen=null;
	bb_screenmanager_ScreenManager.g_ActiveScreenName="";
	bb_controls_Controls.g_ControlMethod=0;
	bb_ld_LDApp.g_RefreshRate=0;
	bb_ld_LDApp.g_Delta=.0;
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
	bb_ld_LDApp.g_TargetScreenX=0.0;
	bb_ld_LDApp.g_ActualScreenX=0;
	bb_ld_LDApp.g_ScreenMoveRate=0.4;
	bb_ld_LDApp.g_TargetScreenY=0.0;
	bb_ld_LDApp.g_ActualScreenY=0;
	bb_ld_LDApp.g_ScreenX=0;
	bb_ld_LDApp.g_ScreenY=0;
	bb_screenmanager_ScreenManager.g_NextScreenName="";
	bb_entity_Entity.g_a=[];
	bb_ld_LDApp.g_level=null;
	bb_dog_Dog.g_a=[];
	bb_dog_Dog.g_gfxStandFront=null;
	bb_yeti_Yeti.g_a=[];
	bb_yeti_Yeti.g_gfxStandFront=null;
	bb_yeti_Yeti.g_gfxRunFront=null;
	bb_yeti_Yeti.g_gfxRunLeft=null;
	bb_yeti_Yeti.g_gfxRunRight=null;
	bb_yeti_Yeti.g_gfxShadow=null;
	bb_skier_Skier.g_a=[];
	bb_skier_Skier.g_gfxSki=null;
	bb_skier_Skier.g_gfxTease=null;
	bb_skier_Skier.g_gfxShadow=null;
	bb_skier_Skier.g_gfxFall=null;
	bb_tree_Tree.g_a=[];
	bb_tree_Tree.g_gfxSmallTree=null;
	bb_tree_Tree.g_gfxBigTree=null;
	bb_tree_Tree.g_gfxTreeStump=null;
	bb_rock_Rock.g_a=[];
	bb_rock_Rock.g_gfxRockBoulder=null;
	bb_rock_Rock.g_gfxRockSpikey=null;
	bb_rock_Rock.g_gfxRockFlat=null;
	bb_flag_Flag.g_a=[];
	bb_flag_Flag.g_gfxFlag=null;
	bb_flag_Flag.g_NextFlag=0;
	bb_yeti_Yeti.g_NextYeti=0;
	bb_dog_Dog.g_NextDog=0;
	bb_skier_Skier.g_NextSkier=0;
	bb_level_Level.g_Width=1000;
	bb_random_Seed=1234;
	bb_level_Level.g_Height=17500;
	bb_tree_Tree.g_NextTree=0;
	bb_rock_Rock.g_NextRock=0;
	bb_sfx_SFX.g_MusicActive=true;
	bb_sfx_SFX.g_CurrentMusic="";
}
//${TRANSCODE_END}
