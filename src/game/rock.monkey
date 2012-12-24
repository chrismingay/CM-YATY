Import ld

Class Rock Extends Entity

	Global gfxStandFront:Image

	Global a:Rock[]
	Global NextRock:Int
	Const MAX_ROCKS:Int = 10
	
	Function Init:Void(tLev:Level)
		a = New Rock[MAX_ROCKS]
		For Local i:Int = 0 Until MAX_ROCKS
			a[i] = New Rock(tLev)
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(0, 80, 16, 16, 1, Image.MidHandle)
		
		Entity.Register(EntityType.ROCK, a)
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_ROCKS
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_ROCKS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tRock:Int = NextRock
		
		a[NextRock].Activate()
		a[NextRock].SetPosition(tX, tY)
		
		NextRock += 1
		If NextRock = MAX_ROCKS
			NextRock = 0
		EndIf
		
		Return tRock
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