Import ld

Class GameScreen Extends Screen

	Field level:Level
	
	Method OnScreenStart:Void()
		LDApp.level = New Level()
		level = LDApp.level
		
		
	End
	
	Method OnScreenEnd:Void()
		
	End
	
	Method Update:Void()
		
		If level <> Null
			level.Update()
		EndIf
	
	End
	
	Method Render:Void()
		Cls(255, 255, 255)
		If level <> Null
			level.Render()
		End
	End
	
	Method New()
		
	End


End