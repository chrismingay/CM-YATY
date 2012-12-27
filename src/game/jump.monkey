Import ld

Class Jump Extends Entity

	Global gfxJump:Image

	Global a:Jump[]
	Global NextJump:Int
	Const MAX_JUMPS:Int = 50
	
	Function Init:Void(tLev:Level)
		a = New Jump[MAX_JUMPS]
		Entity.a[EntityType.JUMP] = New Entity[MAX_JUMPS]
		For Local i:Int = 0 Until MAX_JUMPS
			a[i] = New Jump(tLev)
			Entity.a[EntityType.JUMP][i] = a[i]
		Next
		
		gfxJump = GFX.Tileset.GrabImage(0, 232, 16, 8, 1, Image.MidHandle)
		
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_JUMPS
			If a[i].Active = True
				a[i].Update()
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_JUMPS
			If a[i].Active = True
				If a[i].IsOnScreen()
					a[i].Render()
				EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tJump:Int = NextJump
		
		a[NextJump].Activate()
		a[NextJump].SetPosition(tX, tY)
		
		NextJump += 1
		If NextJump = MAX_JUMPS
			NextJump = 0
		EndIf
		
		Return tJump
	End
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.JUMP
		W = 20
		H = 10
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
		GFX.Draw(gfxJump,X,Y + Z)	
	End

End