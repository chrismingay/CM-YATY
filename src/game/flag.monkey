Import ld

Class Flag Extends Entity

	Global gfxStandFront:Image

	Global a:Flag[]
	Global NextFlag:Int
	Const MAX_FLAGS:Int = 10
	
	Function Init:Void(tLev:Level)
		a = New Flag[MAX_FLAGS]
		For Local i:Int = 0 Until MAX_FLAGS
			a[i] = New Flag(tLev)
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(0, 80, 16, 16, 1, Image.MidHandle)
		
		Entity.Register(EntityType.FLAG, a)
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_FLAGS
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_FLAGS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tFlag:Int = NextFlag
		
		a[NextFlag].Activate()
		a[NextFlag].SetPosition(tX, tY)
		
		NextFlag += 1
		If NextFlag = MAX_FLAGS
			NextFlag = 0
		EndIf
		
		Return tFlag
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