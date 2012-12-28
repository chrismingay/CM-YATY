Import ld

Class ControlMethodTypes
	Const KEYBOARD:Int = 0
	Const JOYPAD:Int = 1
	Const TOUCH:Int = 2
End

Class Controls

	Global gfxTouchOverlay:Image
	Global gfxTouchEscape:Image
	Global gfxTouchRestart:Image
	
	Global TouchOverlayAlpha:Float = 0.0
	Global TouchOverlayAlphaTarget:Float = 0.0
	
	Global TouchRestartAlpha:Float = 0.0
	Global TouchRestartAlphaTarget:Float = 0.0
	
	
	Global LeftKey:Int = KEY_A
	Global RightKey:Int = KEY_D
	Global UpKey:Int = KEY_W
	Global DownKey:Int = KEY_S
	Global ActionKey:Int = KEY_SPACE
	Global Action2Key:Int = KEY_ENTER
	Global EscapeKey:Int = KEY_ESCAPE
	
	Global LeftHit:Bool
	Global RightHit:Bool
	Global UpHit:Bool
	Global DownHit:Bool
	
	Global LeftDown:Bool
	Global RightDown:Bool
	Global UpDown:Bool
	Global DownDown:Bool
	
	Global ActionHit:Bool
	Global ActionDown:Bool
	
	Global Action2Hit:Bool
	Global Action2Down:Bool
	
	Global EscapeHit:Bool
	Global EscapeDown:Bool
	
	Global ControlMethod:Int
	
	'Global TCMove:MyStick
	'Global TCAction1:TouchButton
	'Global TCAction2:TouchButton
	Global TCEscapeKey:TouchButton
	Global TCLeft:TouchButton
	Global TCRight:TouchButton
	Global TCDown:TouchButton
	Global TCActionKey:TouchButton
	
	Global TCButtons:TouchButton[]
	
	' whether or not we're "touching"
	#if TARGET="android" Then
		Global TouchPoint:Bool[] = New Bool[32]
	#else
		Global TouchPoint:Bool
	#endif
	
	
	Function Init:Void()
		'TCMove = New MyStick()
		'TCMove.SetRing(50, LDApp.ScreenHeight - 50, 40)
		'TCMove.SetStick(0, 0, 15)
		'TCMove.SetDeadZone(0.2)
		'TCMove.SetTriggerDistance(5)
		
		TCLeft = New TouchButton(0, LDApp.ScreenHeight * 0.66, LDApp.ScreenWidth * 0.33, LDApp.ScreenHeight * 0.33)
		TCRight = New TouchButton(LDApp.ScreenWidth * 0.66, LDApp.ScreenHeight * 0.66, LDApp.ScreenWidth * 0.33, LDApp.ScreenHeight * 0.33)
		TCDown = New TouchButton(LDApp.ScreenWidth * 0.33, LDApp.ScreenHeight * 0.66, LDApp.ScreenWidth * 0.33, LDApp.ScreenHeight * 0.33)
		TCEscapeKey = New TouchButton(LDApp.ScreenWidth - 17, 1, 16, 16)
		
		TCActionKey = New TouchButton( (LDApp.ScreenWidth - 84) * 0.5, 50, 84, 16)
		
		TCButtons = New TouchButton[5]
		TCButtons[0] = TCLeft
		TCButtons[1] = TCDown
		TCButtons[2] = TCRight
		TCButtons[3] = TCEscapeKey
		TCButtons[4] = TCActionKey
		
		#if TARGET = "android"
			ControlMethod = ControlMethodTypes.TOUCH
		#elseif TARGET = "xna" Or TARGET = "html5" OR TARGET = "flash"
			ControlMethod = ControlMethodTypes.KEYBOARD
		#endif
		
		gfxTouchEscape = GFX.Tileset.GrabImage(144, 160, 16, 16, 1)
		gfxTouchRestart = GFX.Tileset.GrabImage(160, 160, 84, 16, 1)
		
		gfxTouchOverlay = GFX.Tileset.GrabImage(144, 192, 240, 48, 1)
		
	End
	
	Function Update:Void()
	
		LeftHit = False
		RightHit = False
		DownHit = False
		UpHit = False
		ActionHit = False
		Action2Hit = False
		EscapeHit = False
		
		Select ControlMethod
			Case ControlMethodTypes.KEYBOARD
				UpdateKeyboard()
			Case ControlMethodTypes.JOYPAD
				UpdateJoypad()
			Case ControlMethodTypes.TOUCH
				'UpdateTouchThumbSticks()
				UpdateTouchScreen()
		End
		
	
	
	End

	Function UpdateKeyboard:Void()
		If KeyDown(LeftKey)
			If LeftDown = False
				LeftHit = True
			EndIf
			LeftDown = True
		Else
			LeftDown = False
		End
		
		If KeyDown(RightKey)
			If RightDown = False
				RightHit = True
			EndIf
			RightDown = True
		Else
			RightDown = False
		End
		
		If KeyDown(UpKey)
			If UpDown = False
				UpHit = True
			EndIf
			UpDown = True
		Else
			UpDown = False
		End
		
		If KeyDown(DownKey)
			If DownDown = False
				DownHit = True
			EndIf
			DownDown = True
		Else
			DownDown = False
		End
		
		If KeyDown(ActionKey)
			If ActionDown = False
				ActionHit = True
			EndIf
			ActionDown = True
		Else
			ActionDown = False
		End
		
		If KeyDown(Action2Key)
			If Action2Down = False
				Action2Hit = True
			EndIf
			Action2Down = True
		Else
			Action2Down = False
		End
		
		If KeyDown(EscapeKey)
			If EscapeDown = False
				EscapeHit = True
			EndIf
			EscapeDown = True
		Else
			EscapeDown = False
		End
	End
	
	Function UpdateJoypad:Void()
		If JoyX(0, 0) < - 0.1
			If LeftDown = False
				LeftHit = True
			EndIf
			LeftDown = True
		Else
			LeftDown = False
		End
		
		If JoyX(0, 0) > 0.1
			If RightDown = False
				RightHit = True
			EndIf
			RightDown = True
		Else
			RightDown = False
		End
		
		If JoyY(0, 0) > 0.1
			If UpDown = False
				UpHit = True
			EndIf
			UpDown = True
		Else
			UpDown = False
		End
		
		If JoyY(0, 0) < -0.1
			If DownDown = False
				DownHit = True
			EndIf
			DownDown = True
		Else
			DownDown = False
		End
		
		If JoyDown(JOY_A)
			If ActionDown = False
				ActionHit = True
			EndIf
			ActionDown = True
		Else
			ActionDown = False
		End
		
		If JoyDown(JOY_B)
			If Action2Down = False
				Action2Hit = True
			EndIf
			Action2Down = True
		Else
			Action2Down = False
		End
		
		If JoyDown(JOY_START)
			If EscapeDown = False
				EscapeHit = True
			EndIf
			EscapeDown = True
		Else
			EscapeDown = False
		End
	End
	
'	Function UpdateTouchThumbSticks:Void()
'	
'		TCAction1.Hit = False
'		TCAction2.Hit = False
'		TCEscapeKey.Hit = False
'		
'		#if TARGET="android" Then
'			For Local i:Int = 0 To 31
'				If TouchHit(i) Then
'					TouchPoint[i] = True
'					TCMove.StartTouch(VTouchX(i), VTouchY(i), i)
'					For Local bi:Int = 0 To 2
'						If TCButtons[bi].Check(VTouchX(i), VTouchY(i))
'							TCButtons[bi].Hit = True
'						EndIf
'					Next
'				ElseIf TouchDown(i) Then
'					TCMove.UpdateTouch(VTouchX(i), VTouchY(i), i)
'					For Local bi:Int = 0 To 2
'						If TCButtons[bi].Check(VTouchX(i), VTouchY(i))
'							TCButtons[bi].Down = True
'						EndIf
'					Next
'				ElseIf TouchPoint[i] Then
'					TouchPoint[i] = False
'					TCMove.StopTouch(i)
'					For Local bi:Int = 0 To 2
'						TCButtons[bi].Down = False
'					Next
'				End
'			End
'		#else
'			If MouseHit(0) Then
'				TouchPoint = True
'				TCMove.StartTouch(VMouseX(), VMouseY(), 0)
'				For Local i:Int = 0 To 2
'					If TCButtons[i].Check(VMouseX(), VMouseY())
'						TCButtons[i].Hit = True
'					EndIf
'				Next
'			ElseIf MouseDown(0) Then
'				TCMove.UpdateTouch(VMouseX(), VMouseY(), 0)
'				For Local i:Int = 0 To 2
'					If TCButtons[i].Check(VMouseX(), VMouseY())
'						TCButtons[i].Down = True
'					EndIf
'				Next
'			ElseIf TouchPoint Then
'				TouchPoint = False
'				TCMove.StopTouch(0)
'				For Local i:Int = 0 To 2
'					TCButtons[i].Down = False
'				Next
'			End
'		#endif
'		
'		If TCMove.GetDX() < -0.1
'			If LeftDown = False
'				LeftHit = True
'			EndIf
'			LeftDown = True
'		Else
'			LeftDown = False
'		End
'		
'		If TCMove.GetDX() > 0.1
'			If RightDown = False
'				RightHit = True
'			EndIf
'			RightDown = True
'		Else
'			RightDown = False
'		End
'		
'		If TCMove.GetDY() > 0.1
'			If UpDown = False
'				UpHit = True
'			EndIf
'			UpDown = True
'		Else
'			UpDown = False
'		End
'		
'		If TCMove.GetDY() < -0.1
'			If DownDown = False
'				DownHit = True
'			EndIf
'			DownDown = True
'		Else
'			DownDown = False
'		End
'		
'		If TCAction1.Down
'			If ActionDown = False
'				ActionHit = True
'			EndIf
'			ActionDown = True
'		Else
'			ActionDown = False
'		End
'		
'		If TCAction2.Down
'			If Action2Down = False
'				Action2Hit = True
'			EndIf
'			Action2Down = True
'		Else
'			Action2Down = False
'		End
'		
'		If TCEscapeKey.Down
'			If EscapeDown = False
'				EscapeHit = True
'			EndIf
'			EscapeDown = True
'		Else
'			EscapeDown = False
'		End
'		
'		
'		
'	End
	
	Function UpdateTouchScreen:Void()
	
		For Local bi:Int = 0 To 4
			TCButtons[bi].Hit = False
		Next
	
		If TouchOverlayAlpha > TouchOverlayAlphaTarget - 0.01 And TouchOverlayAlpha < TouchOverlayAlphaTarget + 0.01
			TouchOverlayAlpha = TouchOverlayAlphaTarget
		ElseIf TouchOverlayAlpha < TouchOverlayAlphaTarget - 0.1
			TouchOverlayAlpha += 0.01
		ElseIf TouchOverlayAlpha > TouchOverlayAlphaTarget
			TouchOverlayAlpha -= 0.01
		EndIf
		
		If TouchRestartAlpha > TouchRestartAlphaTarget - 0.01 And TouchRestartAlpha < TouchRestartAlphaTarget + 0.01
			TouchRestartAlpha = TouchRestartAlphaTarget
		ElseIf TouchRestartAlpha < TouchRestartAlphaTarget - 0.1
			TouchRestartAlpha += 0.01
		ElseIf TouchRestartAlpha > TouchRestartAlphaTarget
			TouchRestartAlpha -= 0.01
		EndIf
	
		#if TARGET="android" Then
			For Local i:Int = 0 To 31
				If TouchHit(i) Then
					TouchPoint[i] = True
					For Local bi:Int = 0 To 4
						If TCButtons[bi].Check(VTouchX(i), VTouchY(i))
							TCButtons[bi].Hit = True
						EndIf
					Next
				ElseIf TouchDown(i) Then
					For Local bi:Int = 0 To 4
						If TCButtons[bi].Check(VTouchX(i), VTouchY(i))
							TCButtons[bi].Down = True
						EndIf
					Next
				ElseIf TouchPoint[i] Then
					TouchPoint[i] = False
					For Local bi:Int = 0 To 4
						TCButtons[bi].Down = False
					Next
				End
			End
		#else
			If MouseHit(0) Then
				TouchPoint = True
				For Local i:Int = 0 To 4
					If TCButtons[i].Check(VMouseX(), VMouseY())
						TCButtons[i].Hit = True
					EndIf
				Next
			ElseIf MouseDown(0) Then
				For Local i:Int = 0 To 4
					If TCButtons[i].Check(VMouseX(), VMouseY())
						TCButtons[i].Down = True
					EndIf
				Next
			ElseIf TouchPoint Then
				TouchPoint = False
				For Local i:Int = 0 To 4
					TCButtons[i].Down = False
				Next
			End
		#endif
		
		Controls.LeftHit = TCButtons[0].Hit
		Controls.DownHit = TCButtons[1].Hit
		Controls.RightHit = TCButtons[2].Hit
		Controls.EscapeHit = TCButtons[3].Hit
		Controls.ActionHit = TCButtons[4].Hit
		
	End
	
	Function RenderTouchScreen:Void()
		SetColor(255, 255, 255)
		SetAlpha(1.0)
		DrawImage(Controls.gfxTouchEscape, 223, 1)
		
		SetAlpha(TouchOverlayAlpha)
		DrawImage(Controls.gfxTouchOverlay, 0, 312)
		
		SetAlpha(TouchRestartAlpha)
		DrawImage(Controls.gfxTouchRestart, (LDApp.ScreenWidth - 84) * 0.5, 50)
		
		'For Local i:Int = 0 To 3
			'TCButtons[i].Render()
		'Next
	End
End