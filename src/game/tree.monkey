Import ld

Class Tree Extends Entity

	Global gfxSmallTree:Image
	Global gfxBigTree:Image
	Global gfxTreeStump:Image

	Global a:Tree[]
	Global NextTree:Int
	Const MAX_TREES:Int = 1000
	
	Function Init:Void(tLev:Level)
		a = New Tree[MAX_TREES]
		Entity.a[EntityType.TREE] = New Entity[MAX_TREES]
		For Local i:Int = 0 Until MAX_TREES
			a[i] = New Tree(tLev)
			Entity.a[EntityType.TREE][i] = a[i]
		Next
		
		gfxSmallTree = GFX.Tileset.GrabImage(32, 256, 16, 16, 1, Image.MidHandle)
		gfxBigTree = GFX.Tileset.GrabImage(0, 240, 16, 32, 1, Image.MidHandle)
		gfxTreeStump = GFX.Tileset.GrabImage(16, 256, 16, 16, 1, Image.MidHandle)
		
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
		
		
		Local chance:Float = Rnd()
		Local tType:Int = 0
		If chance < 0.5
			tType = TreeType.BIG
		ElseIf chance < 0.8
			tType = TreeType.SMALL
		Else
			tType = TreeType.STUMP
		EndIf
		
		a[NextTree].Type = tType
		
		NextTree += 1
		If NextTree = MAX_TREES
			NextTree = 0
		EndIf
		
		Return tT
	End
	
	Field Type:Int
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.TREE
		W = 10
		H = 10
	End
	
	Method Activate:Void()
		Super.Activate()
	End
	
	Method Update:Void()
		
	End
	
	Method Render:Void()
		Select Type
			Case TreeType.BIG
				GFX.Draw(gfxBigTree, X, Y)
			Case TreeType.SMALL
				GFX.Draw(gfxSmallTree, X, Y)
			Case TreeType.STUMP
				GFX.Draw(gfxTreeStump, X, Y)
		End
		
	End

End

Class TreeType
	Const BIG:Int = 0
	Const SMALL:Int = 1
	Const STUMP:Int = 2
End