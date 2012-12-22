Import ld

Class ControlMethodTypes
	Const KEYBOARD:Int = 0
	Const JOYPAD:Int = 1
	Const TOUCH:Int = 2
End

Class Controls
	
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
	
	Global ControlMethod:Int = ControlMethodTypes.KEYBOARD
	
	Global TCMove:MyStick
	Global TCAction1:TouchButton
	Global TCAction2:TouchButton
	Global TCEscapeKey:TouchButton
	
	Global TCButtons:TouchButton[]
	
	' whether or not we're "touching"
	#if TARGET="android" Then
		Global TouchPoint:Bool[] = New Bool[32]
	#else
		Global TouchPoint:Bool
	#endif
	
	
	Function Init:Void()
		TCMove = New MyStick()
		TCMove.SetRing(50, LDApp.ScreenHeight - 50, 40)
		TCMove.SetStick(0, 0, 15)
		TCMove.SetDeadZone(0.2)
		TCMove.SetTriggerDistance(5)
		
		TCAction1 = New TouchButton(LDApp.ScreenWidth - 60, LDApp.ScreenHeight - 40, 20, 20)
		TCAction2 = New TouchButton(LDApp.ScreenWidth - 30, LDApp.ScreenHeight - 40, 20, 20)
		TCEscapeKey = New TouchButton(LDApp.ScreenWidth - 20, 0, 20, 20)
		
		TCButtons = New TouchButton[3]
		TCButtons[0] = TCAction1
		TCButtons[1] = TCAction2
		TCButtons[2] = TCEscapeKey
		
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
				UpdateTouch()
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
	
	Function UpdateTouch:Void()
	
		TCAction1.Hit = False
		TCAction2.Hit = False
		TCEscapeKey.Hit = False
		
		#if TARGET="android" Then
			For Local i:Int = 0 To 31
				If TouchHit(i) Then
					TouchPoint[i] = True
					TCMove.StartTouch(VTouchX(i), VTouchY(i), i)
					For Local bi:Int = 0 To 2
						If TCButtons[bi].Check(VTouchX(i), VTouchY(i))
							TCButtons[bi].Hit = True
						EndIf
					Next
				ElseIf TouchDown(i) Then
					TCMove.UpdateTouch(VTouchX(i), VTouchY(i), i)
					For Local bi:Int = 0 To 2
						If TCButtons[bi].Check(VTouchX(i), VTouchY(i))
							TCButtons[bi].Down = True
						EndIf
					Next
				ElseIf TouchPoint[i] Then
					TouchPoint[i] = False
					TCMove.StopTouch(i)
					For Local bi:Int = 0 To 2
						TCButtons[bi].Down = False
					Next
				End
			End
		#else
			If MouseHit(0) Then
				TouchPoint = True
				TCMove.StartTouch(VMouseX(), VMouseY(), 0)
				For Local i:Int = 0 To 2
					If TCButtons[i].Check(VMouseX(), VMouseY())
						TCButtons[i].Hit = True
					EndIf
				Next
			ElseIf MouseDown(0) Then
				TCMove.UpdateTouch(VMouseX(), VMouseY(), 0)
				For Local i:Int = 0 To 2
					If TCButtons[i].Check(VMouseX(), VMouseY())
						TCButtons[i].Down = True
					EndIf
				Next
			ElseIf TouchPoint Then
				TouchPoint = False
				TCMove.StopTouch(0)
				For Local i:Int = 0 To 2
					TCButtons[i].Down = False
				Next
			End
		#endif
		
		If TCMove.GetDX() < -0.1
			If LeftDown = False
				LeftHit = True
			EndIf
			LeftDown = True
		Else
			LeftDown = False
		End
		
		If TCMove.GetDX() > 0.1
			If RightDown = False
				RightHit = True
			EndIf
			RightDown = True
		Else
			RightDown = False
		End
		
		If TCMove.GetDY() > 0.1
			If UpDown = False
				UpHit = True
			EndIf
			UpDown = True
		Else
			UpDown = False
		End
		
		If TCMove.GetDY() < -0.1
			If DownDown = False
				DownHit = True
			EndIf
			DownDown = True
		Else
			DownDown = False
		End
		
		If TCAction1.Down
			If ActionDown = False
				ActionHit = True
			EndIf
			ActionDown = True
		Else
			ActionDown = False
		End
		
		If TCAction2.Down
			If Action2Down = False
				Action2Hit = True
			EndIf
			Action2Down = True
		Else
			Action2Down = False
		End
		
		If TCEscapeKey.Down
			If EscapeDown = False
				EscapeHit = True
			EndIf
			EscapeDown = True
		Else
			EscapeDown = False
		End
		
		
		
	End
End