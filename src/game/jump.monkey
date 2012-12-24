Import ld

Class Jump Extends Entity

	Global gfxStandFront:Image

	Global a:Jump[]
	Global NextJump:Int
	Const MAX_JUMPS:Int = 10
	
	Function Init:Void(tLev:Level)
		a = New Jump[MAX_JUMPS]
		For Local i:Int = 0 Until MAX_JUMPS
			a[i] = New Jump(tLev)
		Next
		
		gfxStandFront = GFX.Tileset.GrabImage(0, 80, 16, 16, 1, Image.MidHandle)
		
		Entity.Register(EntityType.JUMP, a)
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