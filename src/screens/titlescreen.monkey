Import ld

Class TitleScreen Extends Screen
	
	Method OnScreenStart:Void()
		
	End
	
	Method OnScreenEnd:Void()
		
	End
	
	Method Update:Void()
		Rnd()
		If Controls.ActionHit
			ScreenManager.ChangeScreen("game")
		EndIf
	End
	
	Method Render:Void()
		
	End
	
	Method New()
		
	End


End