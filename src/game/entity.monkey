Import ld

Class Entity

	Const SCREEN_PADDING:Int = 50
	
	Field X:Float
	Field Y:Float
	Field XS:Float
	Field YS:Float
	
	Field W:Float
	Field H:Float
	
	Field level:Level
	
	Field Active:Bool
	
	Method New(tLev:Level)
		level = tLev
		W = 16
		H = 16
	End
	
	Method SetPosition:Void(tX:Float, tY:Float)
		X = tX
		Y = tY
	End
	
	Method Update:Void()
		
	End
	
	Method Render:Void()
		
	End
	
	Method Activate:Void()
		Active = True
	End
	
	Method Deactivate:Void()
		Active = False
	End
	
	Method IsOnScreen:Bool()
		Return RectOverRect(X, Y, W, H, LDApp.ScreenX - SCREEN_PADDING, LDApp.ScreenY - SCREEN_PADDING, LDApp.ScreenWidth + (SCREEN_PADDING * 2), LDApp.ScreenHeight + (SCREEN_PADDING * 2))
	End
	
End