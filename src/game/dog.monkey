Import ld

Class Dog Extends Entity

	Global gfxStandFront:Image

	Global a:Dog[]
	Global NextDog:Int
	Const MAX_DOGS:Int = 10
	
	Function Init:Void(tLev:Level)
		a = New Dog[MAX_DOGS]
		Entity.a[EntityType.DOG] = New Entity[MAX_DOGS]
		For Local i:Int = 0 Until MAX_DOGS
			a[i] = New Dog(tLev)
			Entity.a[EntityType.DOG][i] = a[i]
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(0, 80, 16, 16, 1, Image.MidHandle)
		
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_DOGS
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_DOGS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tDog:Int = NextDog
		
		a[NextDog].Activate()
		a[NextDog].SetPosition(tX,tY)
		
		NextDog += 1
		If NextDog = MAX_DOGS
			NextDog = 0
		EndIf
		
		Return tDog
	End
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.DOG
		W = 8
		H = 8
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