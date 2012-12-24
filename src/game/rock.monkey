Import ld

Class Rock Extends Entity

	Global gfxRockBoulder:Image
	Global gfxRockSpikey:Image
	Global gfxRockFlat:Image

	Global a:Rock[]
	Global NextRock:Int
	Const MAX_ROCKS:Int = 300
	
	Function Init:Void(tLev:Level)
		a = New Rock[MAX_ROCKS]
		Entity.a[EntityType.ROCK] = New Entity[MAX_ROCKS]
		For Local i:Int = 0 Until MAX_ROCKS
			a[i] = New Rock(tLev)
			Entity.a[EntityType.ROCK][i] = a[i]
		Next
		
		gfxRockBoulder = GFX.Tileset.GrabImage(0, 288, 16, 16, 1, Image.MidHandle)
		gfxRockSpikey = GFX.Tileset.GrabImage(16, 288, 16, 16, 1, Image.MidHandle)
		gfxRockFlat = GFX.Tileset.GrabImage(32, 288, 16, 16, 1, Image.MidHandle)
		
		
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
		
		Local chance:Float = Rnd()
		Local tType:Int = 0
		If chance < 0.33
			tType = RockType.BOULDER
		ElseIf chance < 0.66
			tType = RockType.SPIKEY
		Else
			tType = RockType.FLAT
		EndIf
		
		a[NextRock].Type = tType
		
		
		a[NextRock].Activate()
		a[NextRock].SetPosition(tX, tY)
		
		NextRock += 1
		If NextRock = MAX_ROCKS
			NextRock = 0
		EndIf
		
		Return tRock
	End
	
	Field Type:Int
	
	Method New(tLev:Level)
		level = tLev
	End
	
	Method Activate:Void()
		Super.Activate()
	End
	
	Method Update:Void()
		
	End
	
	Method Render:Void()
		Select Type
			Case RockType.BOULDER
				GFX.Draw(gfxRockBoulder, X, Y)
			Case RockType.FLAT
				GFX.Draw(gfxRockFlat, X, Y)
			Case RockType.SPIKEY
				GFX.Draw(gfxRockSpikey, X, Y)
		End
	End

End

Class RockType
	Const BOULDER:Int = 0
	Const SPIKEY:Int = 1
	Const FLAT:Int = 2
End