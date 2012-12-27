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
	
	Field Collidable:Bool = True
	
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
		
		If Collidable = False And EnType <> EntityType.YETI And EnType <> EntityType.SKIER
			PostCollidableUpdate()
		EndIf
	End
	
	Method StartPostCollidable:Void()
		XS = Rnd(1.5, 2.5)
		If Rnd() < 0.5
			XS = 0 - XS
		EndIf
		
		YS = Rnd(-2, 0.5)
		
		ZS = 0 - Rnd(2, 4)
		Z = ZS
		
		
		Collidable = False
	End
	
	Method PostCollidableUpdate:Void()
		X += XS * LDApp.Delta
		Y += YS * LDApp.Delta
		Z += ZS * LDApp.Delta
		
		If onFloor
			If ZS < 0.5
				Deactivate()
				Print "Deactivated " + ZS
			Else
			
				ZS = 0 - (ZS * 0.75)
				XS *= 0.75
				YS *= 0.75
			EndIf
		EndIf
	End
	
	Method Render:Void()
		'SetColor(255, 0, 0)
		'DrawHollowRect(X - (W * 0.5) - LDApp.ScreenX, Y - (H * 0.5) - LDApp.ScreenY, W, H)
		'SetColor(255, 255, 255)
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
	
		If Z < - 2
			Return EntityType.NONE
		End
	
		For Local Type:Int = 0 Until EntityType.COUNT
			If EnType <> Type
				Local l:Int = Entity.a[Type].Length()
				For Local i:Int = 0 Until l
					If Entity.a[Type][i].Active = True And Entity.a[Type][i].Collidable = True
						If RectOverRect(X - (W * 0.5), Y - (H * 0.5), W, H, Entity.a[Type][i].X - (Entity.a[Type][i].W * 0.5), Entity.a[Type][i].Y - (Entity.a[Type][i].H * 0.5), Entity.a[Type][i].W, Entity.a[Type][i].H)
							If EnType = EntityType.YETI
								Entity.a[Type][i].StartPostCollidable()
							EndIf
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