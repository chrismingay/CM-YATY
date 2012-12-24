Import ld

Class Level

	Field controlledYeti:Int = 0
	
	Field txtWait:RazText
	
	Method New()
	
		Entity.Init()
		
		Dog.Init(LDApp.level)
		Yeti.Init(LDApp.level)
		Skier.Init(LDApp.level)
		
		controlledYeti = Yeti.Create(0, 0)
		Yeti.a[controlledYeti].StartWaiting()
		
		Dog.Create(50, 50)
		
		Local firstSkier:Int = Skier.Create(50, -70)
		Skier.a[firstSkier].StartTeasing()
		
		LDApp.SetScreenPosition(Yeti.a[controlledYeti].X, Yeti.a[controlledYeti].Y)
		
		txtWait = New RazText()
		txtWait.AddMutliLines(LoadString("txt/wait.txt").Replace("~n", ""))
		txtWait.SetPos(96, 320)
		txtWait.SetSpacing(-3, -1)
		
		SFX.Music("ambient")
	End

	Method Update:Void()
	
		LDApp.SetScreenTarget(Yeti.a[controlledYeti].X, Yeti.a[controlledYeti].Y + (LDApp.ScreenHeight * 0.25))
	
		Dog.UpdateAll()
		Yeti.UpdateAll()
		Skier.UpdateAll()
		
	End
	
	Method Render:Void()
	
		RenderGui()
	
		Dog.RenderAll()
		Yeti.RenderAll()
		Skier.RenderAll()
		
		' txtWait.Draw()
		
		
	End
	
	Method StartPreChase:Void()
		
	End
	
	Method UpdatePreChase:Void()
		
	End
	
	Method StartChase:Void()
		
	End
	
	Method UpdateChase:Void()
		
	End
	
	Method RenderGui:Void()
		SetColor(255, 255, 255)
		SetAlpha(1.0)
		DrawImageRect(GFX.Tileset, 1, 1, 504, 0, 8, 360)
		DrawImageRect(GFX.Tileset, 6, 1 + (Yeti.a[controlledYeti].Y / 50), 464, 0, 10, 10)
		For Local i:Int = 0 Until Skier.MAX_SKIERS
			If Skier.a[i].Active = True
				DrawImageRect(GFX.Tileset, 6, 1 + (Skier.a[i].Y / 50), 480, 0, 10, 10)
			EndIf
		Next
		
		
	End
	
End

Class LevelStatusType
	Const PRE_CHASE:Int = 0
	Const CHASING:Int = 1
	Const TOO_EARILY:Int = 2
End