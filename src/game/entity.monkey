Import ld

Class Entity

	Const SCREEN_PADDING:Int = 50
	
	Field X:Float
	Field Y:Float
	Field Z:Float
	Field XS:Float
	Field YS:Float
	Field ZS:Float
	
	Field onFloor:Bool
	
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
		If Z >= 0
			onFloor = True
			Z = 0
		Else
			onFloor = False
		End
	End
	
	Method Render:Void()
		
	End
	
	Method Activate:Void()
		Active = True
	End
	
	Method Deactivate:Void()
		Active = False
	End
	
	Method IsOnScreen:Bool(tAdditionalBuffer:Int = 0.0)
		Return RectOverRect(X, Y, W, H, LDApp.ScreenX - SCREEN_PADDING - tAdditionalBuffer, LDApp.ScreenY - SCREEN_PADDING - tAdditionalBuffer, LDApp.ScreenWidth + (SCREEN_PADDING * 2) + (tAdditionalBuffer * 2), LDApp.ScreenHeight + (SCREEN_PADDING * 2) + (tAdditionalBuffer * 2))
	End
	
End

Class EntityMoveDirection
	Const L:Int = 0
	Const LLD:Int = 1
	Const LDD:Int = 2
	Const D:Int = 3
	Const RDD:Int = 4
	Const RRD:Int = 5
	Const R:Int = 6
	
End