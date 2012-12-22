Strict

Import mojo

' FRAMEWORK IMPORTS
Import src.framework.autofit
'Import src.framework.camera
Import src.framework.controls
Import src.framework.functions
Import src.framework.gfx
Import src.framework.raztext
Import src.framework.rect
Import src.framework.screen
Import src.framework.screenmanager
Import src.framework.sfx
Import src.framework.touchbutton
Import src.framework.virtualstick

' GAME IMPORTS
Import src.game.bump
Import src.game.dog
Import src.game.entity
Import src.game.flag
Import src.game.jump
Import src.game.level
Import src.game.rock
Import src.game.skier
Import src.game.snowboarder
Import src.game.tree
Import src.game.yeti

' SCREEN IMPORTS
Import src.screens.creditsscreen
Import src.screens.gamescreen
Import src.screens.logoscreen
Import src.screens.optionsscreen
Import src.screens.postgamescreen
Import src.screens.pregamescreen
Import src.screens.titlescreen
Import src.screens.waitjoypadscreen

#XNA_WINDOW_WIDTH=480
#XNA_WINDOW_HEIGHT=720
#MOJO_IMAGE_FILTERING_ENABLED=false

Class LDApp Extends App

	Global level:Level

	Global ScreenWidth:Int = 240
	Global ScreenHeight:Int = 360
	Global ScreenX:Int = 0
	Global ScreenY:Int = 0
	
	Global TargetScreenX:Float = 0
	Global TargetScreenY:Float = 0
	
	Global ActualScreenX:Int = 0
	Global ActualScreenY:Int = 0
	
	Global ScreenMoveRate:Float = 0.1
	
	Global RefreshRate:Int
	Global Delta:Float
	
	Function SetScreenTarget:Void(tX:Float, tY:Float)
		TargetScreenX = tX - (ScreenWidth * 0.5)
		TargetScreenY = tY - (ScreenHeight * 0.5)
	End
	
	
	Function SetScreenPosition:Void(tX:Float, tY:Float)
		SetScreenTarget(tX, tY)
		ActualScreenX = TargetScreenX
		ActualScreenY = TargetScreenY
	End
	
	
	Method OnCreate:Int()
		
		GFX.Init()
		ScreenManager.Init()
		SFX.Init()
		Controls.Init()
		
		
		RazText.SetTextSheet(LoadImage("gfx/fonts.png"))
		
		' Add the graphics
		' Use C:\Apps\Aseprite\
		
		' Add the screens
		ScreenManager.AddScreen("game", New GameScreen())
		
		
		' Add the sound effects
		' USE C:\Apps\sfxr\
		
		' Add the music
		' USE C:\Apps\MusicGen\
		
		' Set the initial screen details
		ScreenManager.SetFadeColour(0, 0, 0)
		ScreenManager.SetFadeRate(0.1)
		ScreenManager.SetScreen("game")
		
		If Controls.ControlMethod = ControlMethodTypes.TOUCH
			RefreshRate = 30
		Else
			RefreshRate = 60
		End
		
		SetUpdateRate(RefreshRate)
		Delta = RefreshRate / 60.0
		
		
		SetVirtualDisplay(ScreenWidth, ScreenHeight)
		
		Return 0
	End
	
	Method OnUpdate:Int()
	
		Controls.Update()
	
		ActualScreenX += ( (TargetScreenX - ActualScreenX) * ScreenMoveRate)
		ActualScreenY += ( (TargetScreenY - ActualScreenY) * ScreenMoveRate)
		
		ScreenX = Int(ActualScreenX)
		ScreenY = Int(ActualScreenY)
		
		ScreenManager.Update()
		Return 0
	End
	
	Method OnRender:Int()
	
		UpdateVirtualDisplay()
	
		Cls
		ScreenManager.Render()
		Return 0
	End

End

Function Main:Int()
	New LDApp
	Return 0
End

