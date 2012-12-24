Import ld

Class SnowBoarder Extends Entity

	Global gfxStandFront:Image

	Global a:SnowBoarder[]
	Global NextSnowBoarder:Int
	Const MAX_SNOWBOARDER:Int = 10
	
	Function Init:Void(tLev:Level)
		a = New SnowBoarder[MAX_SNOWBOARDER]
		For Local i:Int = 0 Until MAX_SNOWBOARDER
			a[i] = New SnowBoarder(tLev)
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(0, 80, 16, 16, 1, Image.MidHandle)
		
		Entity.Register(EntityType.SNOWBOARDER, a)
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_SNOWBOARDER
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_SNOWBOARDER
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tSB:Int = NextJump
		
		a[NextSnowBoarder].Activate()
		a[NextSnowBoarder].SetPosition(tX, tY)
		
		NextSnowBoarder += 1
		If NextSnowBoarder = MAX_SNOWBOARDER
			NextSnowBoarder = 0
		EndIf
		
		Return tSB
	End
	
	Method New(tLev:Level)
		level = tLev
	End
	
	Method Activate:Void()
		Super.Activate()
	End
	
	Method Update:Void()
		
		If Not IsOnScreen()
			Deactivate()
		EndIf
	
	End
	
	Method Render:Void()
		GFX.Draw(gfxStandFront,X,Y)	
	End

End