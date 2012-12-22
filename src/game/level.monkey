Import ld

Class Level

	Field controlledYeti:Int = 0
	
	Field txtWait:RazText
	
	Method New()
		
		Dog.Init(LDApp.level)
		Yeti.Init(LDApp.level)
		
		controlledYeti = Yeti.Create(0, 0)
		
		Dog.Create(50,50)
		
		LDApp.SetScreenPosition(Yeti.a[controlledYeti].X, Yeti.a[controlledYeti].Y)
		
		txtWait = New RazText()
		txtWait.AddMutliLines(LoadString("txt/wait.txt").Replace("~n", ""))
		txtWait.SetPos(96, 320)
		txtWait.SetSpacing(-3, -1)
	End

	Method Update:Void()
	
		LDApp.SetScreenTarget(Yeti.a[controlledYeti].X, Yeti.a[controlledYeti].Y)
	
		Dog.UpdateAll()
		Yeti.UpdateAll()
		
	End
	
	Method Render:Void()
		Dog.RenderAll()
		Yeti.RenderAll()
		txtWait.Draw()
	End
	
End