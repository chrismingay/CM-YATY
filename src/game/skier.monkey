Import ld

Class Skier Extends Entity

	Global gfxSki:Image
	Global gfxTease:Image
	Global gfxShadow:Image
	Global gfxFall:Image
	
	
	Global a:Skier[]
	Const MAX_SKIERS:Int = 1
	Global NextSkier:Int
	
	
	Function Init:Void(tLev:Level)
		a = New Skier[MAX_SKIERS]
		Entity.a[EntityType.SKIER] = New Entity[MAX_SKIERS]
		For Local i:Int = 0 Until MAX_SKIERS
			a[i] = New Skier(tLev)
			Entity.a[EntityType.SKIER][i] = a[i]
		Next
		
		gfxSki = GFX.Tileset.GrabImage(0, 128, 16, 32, 7, Image.MidHandle)
		gfxTease = GFX.Tileset.GrabImage(112, 128, 16, 32, 2, Image.MidHandle)
		gfxShadow = GFX.Tileset.GrabImage(0, 160, 16, 4, 1, Image.MidHandle)
		gfxFall = GFX.Tileset.GrabImage(144, 128, 16, 32, 4, Image.MidHandle)
		
	End
	
	Function UpdateAll:Void()
		For Local i:Int = 0 Until MAX_SKIERS
			If a[i].Active = True
				'If a[i].IsOnScreen()
					a[i].Update()
				'EndIf
			End
		Next
	End
	
	Function RenderAll:Void()
		For Local i:Int = 0 Until MAX_SKIERS
			If a[i].Active = True
				'If a[i].IsOnScreen()
					a[i].Render()
				'Else
					a[i].RenderMarker()
				'EndIf
			End
		Next
	End
	
	Function Create:Int(tX:Float, tY:Float)
	
		Local tI:Int = NextSkier
		a[tI].Activate()
		a[tI].SetPosition(tX, tY)
		a[tI].Collidable = True
		NextSkier += 1
		If NextSkier >= MAX_SKIERS
			NextSkier = 0
		EndIf
		
		Return tI
		
	End
	
	Field D:Int
	Field Status:Int
	
	Field MaxYS:Float
	Field TargetXS:Float
	
	Field TargetYeti:Int
	
	Field teasingFrame:Int = 0
	Field teasingFrameTimer:Float
	
	Field fallFrame:Int
	Field fallFrameTimer:Float
	
	Field fallTimer:Float
	
	Field TargetFlag:Int = 0
	
	Const MAX_YS:Float = 10.0
	
	Method New(tLev:Level)
		level = tLev
		EnType = EntityType.SKIER
		W = 12
		H = 16
		TargetYeti = 0
	End
	
	Method Update:Void()
	
		Super.Update()
	
		Select Status
		Case SkierStatusType.DAZED
			UpdateDazed()
		Case SkierStatusType.DEAD
			UpdateDead()
		Case SkierStatusType.FALLING
			UpdateFalling()
		Case SkierStatusType.SKIING
			UpdateSkiing()
		Case SkierStatusType.TEASING
			UpdateTeasing()
		Case SkierStatusType.PRE_TEASING
			UpdatePreTeasing()
		End
	
		'If Not IsOnScreen(1000)
		'	Deactivate()
		'EndIf
	End
	
	Method StartSkiing:Void()
	
		Status = SkierStatusType.SKIING
		Z = 0
		D = EntityMoveDirection.D
		YS = 4.0
		XS = 0
		ZS = -2.0
		
		SFX.Music("chase")
		SFX.Play("SkiStart")
	
	End
	
	Method ContinueSkiing:Void()
	
		Status = SkierStatusType.SKIING
		Z = 0
		D = EntityMoveDirection.D
		ZS = 0.0
		
	End
	
	Method UpdateSkiing:Void()
	
		If Rnd() < 0.02
		
			If onFloor
				Local tR:Float = Rnd()
				If tR < 0.33
					SFX.Play("SkiTurn1", SFX.VolumeFromPosition(X, Y), SFX.PanFromPosition(X, Y), Rnd(0.8, 1.2))
				ElseIf tR < 0.66
					SFX.Play("SkiTurn2", SFX.VolumeFromPosition(X, Y), SFX.PanFromPosition(X, Y), Rnd(0.8, 1.2))
				Else
					SFX.Play("SkiTurn3", SFX.VolumeFromPosition(X, Y), SFX.PanFromPosition(X, Y), Rnd(0.8, 1.2))
				End
			EndIf
		
			If X < 0 - (Level.Width * 0.5)
				
				D += 1
			
			ElseIf X > (Level.Width * 0.5)
			
				D -= 1
			
			Else
			
				If Rnd() < 0.5
					D -= 1
				Else
					D += 1
				EndIf
			
			EndIf
			
		EndIf
		
		
		D = Clamp(D, EntityMoveDirection.L + 1, EntityMoveDirection.R - 1)
		
	
	
		If onFloor
			Select D
				Case EntityMoveDirection.L
					MaxYS = 0.0
					TargetXS = 0.0
				Case EntityMoveDirection.LLD
					MaxYS = 1.0
					TargetXS = -2.0
				Case EntityMoveDirection.LDD
					MaxYS = 2.0
					TargetXS = -1.0
				Case EntityMoveDirection.D
					MaxYS = 3.2
					TargetXS = 0.0
				Case EntityMoveDirection.RDD
					MaxYS = 2.0
					TargetXS = 1.0
				Case EntityMoveDirection.RRD
					MaxYS = 1.0
					TargetXS = 2.0
				Case EntityMoveDirection.R
					MaxYS = 0.0
					TargetXS = 0.0
			End
			
			If YS > MaxYS - 0.05 And YS < MaxYS + 0.05
				YS = MaxYS
			ElseIf YS > MaxYS
				YS *= (1.0 - (0.02 * LDApp.Delta))
			ElseIf YS < MaxYS
				YS += (0.05 * LDApp.Delta)
			EndIf
			
			If XS > TargetXS - (0.05 * LDApp.Delta) And XS < TargetXS + (0.05 * LDApp.Delta)
				XS = TargetXS
			ElseIf XS < TargetXS
				XS += 0.05 * LDApp.Delta
			ElseIf XS > TargetXS
				XS -= 0.05 * LDApp.Delta
			EndIf
		
		Else
			ZS += (0.05 * LDApp.Delta)
		End
		
		X += XS * LDApp.Delta
		Y += YS * LDApp.Delta
		Z += ZS * LDApp.Delta
		
		Local colStatus:Int = CheckForCollision()
		
		Select colStatus
		Case EntityType.BUMP
			ZS = -1
		Case EntityType.DOG
			ZS = -1
			XS *= 0.9
			YS *= 0.9
		Case EntityType.FLAG
			XS *= 0.9
			YS *= 0.9
		Case EntityType.JUMP
			XS *= 2
			YS *= 2
			ZS = -3
		Case EntityType.ROCK
			StartFalling()
			XS *= 0.5
			YS *= 0.5
		Case EntityType.SNOWBOARDER
			
		Case EntityType.TREE
			StartFalling()
			XS *= 0.7
			YS *= 0.7
		Case EntityType.YETI
			' TODO END GAME
		End
		
	End
	
	Method StartFalling:Void()
		Local tZ:Float = (Abs(XS) + Abs(YS)) * 0.5
		ZS = 0 - tZ
		Status = SkierStatusType.FALLING
		fallTimer = 0.0
		
		SFX.Play("SkiFall", SFX.VolumeFromPosition(X, Y), SFX.PanFromPosition(X, Y), Rnd(0.8, 1.2))
	End
	
	Method UpdateFalling:Void()
	
		fallTimer += (1.0 * LDApp.Delta)
	
		fallFrameTimer += (1.0 * LDApp.Delta)
		If fallFrameTimer >= 7.0
			fallFrameTimer = 0.0
			fallFrame += 1
			If fallFrame = 4
				fallFrame = 0
			EndIf
		EndIf
		
		If onFloor = True
			If ZS >= 4.0
				ZS = 0 - (ZS * 0.5)
				XS = XS * 0.6
				YS = YS * 0.6
			ElseIf fallTimer >= 15.0
				ContinueSkiing()
			Else
				
			End
		EndIf
		
		X += XS * LDApp.Delta
		Y += YS * LDApp.Delta
		Z += ZS * LDApp.Delta
		
		ZS += (0.05 * LDApp.Delta)
		
	End
	
	Method StartDazed:Void()
		
	End
	
	Method UpdateDazed:Void()
		
	End
	
	Method StartDead:Void()
		
	End
	
	Method UpdateDead:Void()
		
	End
	
	Method StartTeasing:Void()
		Status = SkierStatusType.TEASING
		D = EntityMoveDirection.D
		TargetYeti = FindNearestYeti()
		SFX.Play("SkiStop")
	End
	
	Method UpdateTeasing:Void()
	
		If YS = 0
			If teasingFrameTimer > 0
				teasingFrameTimer -= 1.0 * LDApp.Delta
				teasingFrame = 1
			Else
				teasingFrameTimer = 0
				teasingFrame = 0
			End
		
			If Rnd() < 0.04
				Y += 1
				teasingFrameTimer = 5.0
				SFX.Play("SnowStep4", Rnd(0.2, 0.4), 0.0, Rnd(0.9, 1.1))
			EndIf
		Else
			Y += (YS * LDApp.Delta)
		EndIf
		
		
		If YS <= 0.1
			YS = 0
		Else
			YS *= (1.0 - (0.05 * LDApp.Delta))
		EndIf
		
		
		
		If Yeti.a[TargetYeti].Y - Y < 2
			StartSkiing()
		ElseIf Yeti.a[TargetYeti].Y - Y < 50
			If Rnd() < 0.02
				StartSkiing()
			EndIf
		EndIf
	End
	
	Method StartPreTeasing:Void()
		Status = SkierStatusType.PRE_TEASING
		D = EntityMoveDirection.D
		YS = 1.6
		
		
	End
	
	Method UpdatePreTeasing:Void()
	
		Y += (YS * LDApp.Delta)
	
		If TargetYeti >= 0
			If Yeti.a[TargetYeti].Y - Y < 100
					D = EntityMoveDirection.L
					StartTeasing()
			EndIf
		EndIf
		
	End
	
	Method Render:Void()
	
		Super.Render()
	
		'GFX.Draw(gfxSkiL, X, Y + Z, 0)
		If Z < 0
			SetAlpha(0.25)
			GFX.Draw(gfxShadow, X - (Z * 1), Y + 7 - (Z * 2))
		End
		SetAlpha(1.0)
		
		Select Status
			Case SkierStatusType.TEASING
				GFX.Draw(gfxTease, X, Y + Z, teasingFrame)
			Case SkierStatusType.PRE_TEASING
				GFX.Draw(gfxSki, X, Y + Z, D)
			Case SkierStatusType.FALLING
				GFX.Draw(gfxFall, X, Y + Z, fallFrame)
			Default
				GFX.Draw(gfxSki, X, Y + Z, D)
		End
		
	End
	
	Method RenderMarker:Void()
		Local dX:Int = (X - LDApp.ScreenX)
		Local dY:Int = (Y - LDApp.ScreenY)
		Local hState:Int = 0
		Local vState:Int = 0
		If dX < 0
			hState = -1
		ElseIf dX > LDApp.ScreenWidth
			hState = 1
		EndIf
		If dY < 0
			vState = -1
		ElseIf dY > LDApp.ScreenHeight
			vState = 1
		EndIf
		
		Select hState
			Case -1
			
				Select vState
					Case -1
						DrawImageRect(GFX.Tileset, 1, 1, 16, 192, 8, 8)
					Case 0
						DrawImageRect(GFX.Tileset, 1, dY - 4, 0, 176, 8, 9)
					Case 1
						DrawImageRect(GFX.Tileset, 1, LDApp.ScreenHeight - 9, 0, 192, 8, 8)
				End
			
			Case 0
			
				Select vState
					Case -1
						DrawImageRect(GFX.Tileset, dX - 4, 1, 32, 176, 9, 8)
					Case 0
					
					Case 1
						DrawImageRect(GFX.Tileset, dX - 4, LDApp.ScreenHeight - 10, 16, 176, 9, 8)
				End
			
			Case 1
			
				Select vState
					Case -1
						DrawImageRect(GFX.Tileset, LDApp.ScreenWidth - 9, 1, 24, 192, 8, 8)
					Case 0
						DrawImageRect(GFX.Tileset, LDApp.ScreenWidth - 9, dY - 4, 8, 176, 8, 9)
					Case 1
						DrawImageRect(GFX.Tileset, LDApp.ScreenWidth - 9, LDApp.ScreenHeight - 9, 8, 192, 8, 8)
				End
		End
		
	End
	
	Method Activate:Void()
		Super.Activate()
		TargetYeti = -1
	End
	
	Method FindNearestYeti:Int()
		Local nIndex:Int = -1
		Local nDistance:Float = 99999999
		
		For Local i:Int = 0 Until Yeti.MAX_YETIS
			If Yeti.a[i].Active = True
				If Yeti.a[i].Y > Y
					Local tDist:Float = Yeti.a[i].Y - Y
					If tDist < 200
						If tDist < nDistance
							nIndex = i
							nDistance = tDist
						End
					End
				EndIf
			EndIf
		Next
		
		Return nIndex
	End
	

End

Class SkierStatusType
	Const SKIING:Int = 0
	Const FALLING:Int = 1
	Const DAZED:Int = 2
	Const DEAD:Int = 3
	Const TEASING:Int = 4
	Const PRE_TEASING:Int = 5
End