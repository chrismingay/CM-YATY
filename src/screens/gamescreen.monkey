Import ld

Class GameScreen Extends Screen

	Field level:Level
	
	Method OnScreenStart:Void()
		LDApp.level = GenerateLevel()
		level = LDApp.level
		
		level.Start()
		
		Controls.TouchOverlayAlpha = 0.0
		Controls.TouchOverlayAlphaTarget = 0.0
		
		Controls.TouchRestartAlpha = 0.0
		Controls.TouchRestartAlphaTarget = 0.0
		
	End
	
	Method OnScreenEnd:Void()
		
	End
	
	Method Update:Void()
		
		If level <> Null
			level.Update()
		EndIf
		
		If Controls.EscapeHit
			ScreenManager.SetFadeRate(0.1)
			ScreenManager.ChangeScreen("exit")
		EndIf
	
	End
	
	Method Render:Void()
		Cls(255, 255, 255)
		If level <> Null
			level.Render()
		End
		If Controls.ControlMethod = ControlMethodTypes.TOUCH
			Controls.RenderTouchScreen()
		EndIf
	End
	
	Method New()
		
	End


End