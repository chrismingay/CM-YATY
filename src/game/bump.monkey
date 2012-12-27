Import ld

Class Bump Extends Entity

	Global gfxBump:Image

	Global a:Bump[]
	Global NextBump:Int
	Const MAX_BUMPS:Int = 200
	
	Function Init:Void(tLev:Level)
		a = New Bump[MAX_BUMPS]
		Entity.a[EntityType.BUMP] = New Entity[MAX_BUMPS]
		For Local i:Int = 0 Until MAX_BUMPS
			a[i] = New Bump(tLev)
			Entity.a[EntityType.BUMP][i] = a[i]
		Next
		
		gfxBump = GFX.Tileset.GrabImage(0, 208, 16, 16, 2, Image.MidHandle)
		
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
	
	Field Frame:Int
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.BUMP
		W = 16
		H = 8
		
		If Rnd() < 0.5
			Frame = 0
		Else
			Frame = 1
		EndIf
	End
	
	Method Activate:Void()
		Super.Activate()
	End
	
	Method Update:Void()
	
		Super.Update()
		
		'If Not IsOnScreen()
		'	Deactivate()
		'EndIf
	
	End
	
	Method Render:Void()
		GFX.Draw(gfxBump, X, Y, Frame)
	End

End