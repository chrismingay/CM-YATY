Import ld

Class Bump Extends Entity

	Global gfxStandFront:Image

	Global a:Bump[]
	Global NextBump:Int
	Const MAX_BUMPS:Int = 10
	
	Function Init:Void(tLev:Level)
		a = New Bump[MAX_BUMPS]
		For Local i:Int = 0 Until MAX_BUMPS
			a[i] = New Bump(tLev)
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(0, 80, 16, 16, 1, Image.MidHandle)
		
		Entity.Register(EntityType.BUMP, a)
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_BUMPS
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_BUMPS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tBump:Int = NextBump
		
		a[NextBump].Activate()
		a[NextBump].SetPosition(tX, tY)
		
		NextBump += 1
		If NextBump = MAX_BUMPS
			NextBump = 0
		EndIf
		
		Return tBump
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