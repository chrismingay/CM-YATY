Import ld

Class ScreenManager

	Global gameScreen:GameScreen

	Global Screens:StringMap<Screen>
	Global ActiveScreen:Screen
	Global ActiveScreenName:String
	Global NextScreenName:String
	
	Global FadeAlpha:Float
	Global FadeRate:Float
	Global FadeRed:Float
	Global FadeGreen:Float
	Global FadeBlue:Float
	
	Const FADE_IN:Int = 0
	Const WAITING:Int = 1
	Const FADE_OUT:Int = 2
	Global FadeMode:Int
	
	
	Function Init:Void()
		Screens = New StringMap<Screen>
		
		FadeAlpha = 0.0
		FadeRate = 0.01
		FadeRed = 0.0
		FadeGreen = 0.0
		FadeBlue = 0.0
		
		FadeMode = FADE_IN
		
	End
	
	Function Update:Void()
	
		If ActiveScreen <> Null
			ActiveScreen.Update()
		EndIf
		
		Select FadeMode
			Case FADE_IN
				FadeAlpha -= FadeRate
				If FadeAlpha <= 0.0
					FadeAlpha = 0.0
					FadeMode = WAITING
					
				EndIf
			Case WAITING
			
			Case FADE_OUT
				FadeAlpha += FadeRate
				If FadeAlpha >= 1.0
					FadeAlpha = 1.0
					FadeMode = FADE_IN
					If ActiveScreen <> Null
						ActiveScreen.OnScreenEnd()
					End
					ActiveScreenName = NextScreenName
					ActiveScreen = Screens.Get(ActiveScreenName)
					ActiveScreen.OnScreenStart()
				EndIf
		End
	
	End
	
	Function Render:Void()
		
		SetColor(255, 255, 255)
		SetAlpha(1.0)
	
		If ActiveScreen <> Null
			ActiveScreen.Render()
		EndIf
		
		SetColor(255, 255, 255)
		SetAlpha(1.0)
		If Controls.ControlMethod = ControlMethodTypes.TOUCH
			Controls.TCMove.DoRenderRing()
			Controls.TCMove.DoRenderStick()
			
			Controls.TCAction1.Render()
			Controls.TCAction2.Render()
			Controls.TCEscapeKey.Render()
		EndIf
		
		If FadeMode <> WAITING
			SetColor(FadeRed, FadeGreen, FadeBlue)
			SetAlpha(FadeAlpha)
			DrawRect(0,0,DeviceWidth(),DeviceHeight())
		EndIf
	End
	
	Function AddScreen:Void(tName:String, tScreen:Screen)
		Screens.Set(tName, tScreen)
		If tName = "game"
			gameScreen = GameScreen(tScreen)
		EndIf
	End
	
	Function SetScreen:Void(tName:String)
	
		If ActiveScreen <> Null
			ActiveScreen.OnScreenEnd()
		End
		ActiveScreen = Screens.Get(tName)
		ActiveScreen.OnScreenStart()
		FadeMode = WAITING
		ActiveScreenName = tName
	End
	
	Function ChangeScreen:Void(tName:String)
		If tName <> ActiveScreenName
			If Screens.Contains(tName)
				NextScreenName = tName
				FadeMode = FADE_OUT
			EndIf
		EndIf
	End
	
	Function SetFadeRate:Void(tRate:Float)
		FadeRate = Clamp(tRate, 0.001, 1.0)
	End
	
	Function SetFadeColour:Void(tR:Float, tG:Float, tB:Float)
		FadeRed = Clamp(tR, 0.0, 255.0)
		FadeGreen = Clamp(tG, 0.0, 255.0)
		FadeBlue = Clamp(tB, 0.0, 255.0)
	End
	
End
