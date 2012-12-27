Import ld

Class Flag Extends Entity

	Global gfxFlag:Image

	Global a:Flag[]
	Global NextFlag:Int
	Const MAX_FLAGS:Int = 150
	
	Function Init:Void(tLev:Level)
		a = New Flag[MAX_FLAGS]
		Entity.a[EntityType.FLAG] = New Entity[MAX_FLAGS]
		For Local i:Int = 0 Until MAX_FLAGS
			a[i] = New Flag(tLev)
			Entity.a[EntityType.FLAG][i] = a[i]
		Next
		
		gfxFlag = GFX.Tileset.GrabImage(0, 272, 16, 16, 2, Image.MidHandle)
		
		NextFlag = 0
		
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
	
	Field Type:Int
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.FLAG
		W = 8
		H = 16
	End
	
	Method Activate:Void()
		Super.Activate()
	End
	
	Method Update:Void()
	
		Super.Update()
		
	End
	
	Method Render:Void()
	
		Super.Render()
	
		GFX.Draw(gfxFlag, X, Y + Z, Type)
	End

End

Class FlagType
	Const LEFT_SIDE:Int = 0
	Const RIGHT_SIDE:Int = 1
End