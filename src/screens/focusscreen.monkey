Import ld

Class FocusScreen Extends Screen

	Field focusText:RazText
	
	Method New()
		focusText = New RazText("Click to Focus")
		focusText.SetPos(65, 150)
	End
	
	Method OnScreenStart:Void()
		
	End
	
	Method OnScreenEnd:Void()
		
	End
	
	Method Update:Void()
		If MouseHit(MOUSE_LEFT)
			ScreenManager.ChangeScreen("game")
		EndIf
	End
	
	Method Render:Void()
		Cls(48, 48, 48)
		focusText.Draw()
	End
	


End