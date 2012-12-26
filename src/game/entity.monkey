Import ld



Class EntityType
	Const NONE:Int = -1
	Const BUMP:Int = 0
	Const DOG:Int = 1
	Const FLAG:Int = 2
	Const JUMP:Int = 3
	Const ROCK:Int = 4
	Const SKIER:Int = 5
	Const SNOWBOARDER:Int = 6
	Const TREE:Int = 7
	Const YETI:Int = 8
	
	
	
	Const COUNT:Int = 9
End

Class Entity

	Global a:Entity[][]
	
	Function Init:Void()
		a = New Entity[EntityType.COUNT][]
		For Local i:Int = 0 Until EntityType.COUNT
			'a[i] = New Entity[50]
		Next
	End
	

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
	
	Field EnType:Int
	
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
	
	Method CheckForCollision:Int()
	
		If Z < - 2 Then Return EntityType.NONE
	
		For Local Type:Int = 0 Until EntityType.COUNT
			If EnType <> Type
				Local l:Int = Entity.a[Type].Length()
				For Local i:Int = 0 Until l
					If Entity.a[Type][i].Active = True
						If RectOverRect(X, Y, W, H, Entity.a[Type][i].X, Entity.a[Type][i].Y, Entity.a[Type][i].W, Entity.a[Type][i].H)
							Return Type
						End
					End
				Next
			EndIf
		Next
		Return EntityType.NONE
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