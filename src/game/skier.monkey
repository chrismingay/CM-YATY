Import ld

Class Skier Extends Entity

	Global gfxSki:Image
	
	
	Global a:Skier[]
	Const MAX_SKIERS:Int = 20
	Global NextSkier:Int
	
	
	Function Init:Void(tLev:Level)
		a = New Skier[MAX_SKIERS]
		For Local i:Int = 0 Until MAX_SKIERS
			a[i] = New Skier(tLev)
		Next
		
		gfxSki = GFX.Tileset.GrabImage(0, 128, 16, 32, 7, Image.MidHandle)
		
		Entity.Register(EntityType.SKIER,a)
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
	
	Const MAX_YS:Float = 10.0
	
	Method New(tLev:Level)
		level = tLev
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
		End
	
		'If Not IsOnScreen(1000)
		'	Deactivate()
		'EndIf
	End
	
	Method StartSkiing:Void()
	
		Status = SkierStatusType.SKIING
		Z = 0
		D = EntityMoveDirection.D
		YS = 3.0
		XS = 0
		ZS = -2.0
	
	End
	
	Method UpdateSkiing:Void()
	
		If Rnd() < 0.02
			
			If Rnd() < 0.5
				D -= 1
			Else
				D += 1
			EndIf
		EndIf
		
		
		D = Clamp(D, EntityMoveDirection.L, EntityMoveDirection.R)
		
	
	
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
					MaxYS = 3.0
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
				YS *= (1.0 - (0.05 * LDApp.Delta))
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
		
	End
	
	Method StartFalling:Void()
		Local tZ:Float = (Abs(XS) + Abs(YS)) * 0.5
		ZS = 0 - tZ
	End
	
	Method UpdateFalling:Void()
		
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
		XS = 0
		YS = 0
		TargetYeti = FindNearestYeti()
	End
	
	Method UpdateTeasing:Void()
		If Rnd() < 0.05
			Y += 1
		EndIf
		
		If Yeti.a[TargetYeti].Y - Y < 2
			StartSkiing()
		ElseIf Yeti.a[TargetYeti].Y - Y < 50
			If Rnd() < 0.02
				StartSkiing()
			EndIf
		EndIf
	End
	
	Method Render:Void()
		'GFX.Draw(gfxSkiL, X, Y + Z, 0)
		GFX.Draw(gfxSki, X, Y + Z, D)
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
						DrawImageRect(GFX.Tileset, dX - 4, 1, 32, 176, 8, 9)
					Case 0
					
					Case 1
						DrawImageRect(GFX.Tileset, dX - 4, LDApp.ScreenHeight - 10, 16, 176, 8, 9)
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
End