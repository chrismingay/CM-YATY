Import ld

Class Tree Extends Entity

	Global gfxStandFront:Image

	Global a:Tree[]
	Global NextTree:Int
	Const MAX_TREES:Int = 10
	
	Function Init:Void(tLev:Level)
		a = New Tree[MAX_TREES]
		For Local i:Int = 0 Until MAX_TREES
			a[i] = New Tree(tLev)
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(0, 80, 16, 16, 1, Image.MidHandle)
		
		Entity.Register(EntityType.TREE, a)
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_TREES
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_TREES
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tT:Int = NextTree
		
		a[NextTree].Activate()
		a[NextTree].SetPosition(tX, tY)
		
		NextTree += 1
		If NextTree = MAX_TREES
			NextTree = 0
		EndIf
		
		Return tT
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