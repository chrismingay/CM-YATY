Import ld

Class RazText

	Global TextSheet:Image
	
	Function SetTextSheet:Void(tImage:Image)
		TextSheet = tImage
	End

	Field OriginalString:String
	
	Field CharacterOriginX:Int = 0
	Field CharacterOriginY:Int = 0
	Field CharacterWidth:Int = 10
	Field CharacterHeight:Int = 10
	
	Field Lines:List<RazChar[]>
	
	Field VerticalSpacing:Int = 0
	Field HorizontalSpacing:Int = -2
	
	Field X:Int
	Field Y:Int
	
	Method New()
		Lines = New List<RazChar[] >
	End
	
	Method New(tString:String)
		Lines = New List<RazChar[] >
		OriginalString = tString
		AddLine(tString)
	End
	
	Method Clear:Void()
		Lines.Clear()
	End
	
	Method SetSpacing:Void(tX:Int,tY:Int)
		VerticalSpacing = tY
		HorizontalSpacing = tX
	End
	
	Method SetTextOrigin:Void(tX:Int, tY:Int)
		CharacterOriginX = tX
		CharacterOriginY = tY
	End
	
	Method SetCharacterSize:Void(tW:Int, tH:Int)
		CharacterWidth = tW
		CharacterHeight = tH
	End
	
	Method SetLine:Void(tString:String, cLine:Int = 0)
	
		if cLine = 0
			OriginalString = tString
		EndIf
	
		Local tmp:RazChar[] = New RazChar[tString.Length()]
		For Local i:Int = 0 To tString.Length() - 1
			Local let:Int = tString[i]
			Local letstring:String = tString[i..(i+1)].ToLower()
			Local XOff:Int = 0
			Local YOff:Int = 0
			Select letstring.ToLower()
				Case "a"
					XOff = 0
					YOff = 1
				Case "b"
					XOff = 1
					YOff = 1
				Case "c"
					XOff = 2
					YOff = 1
				Case "d"
					XOff = 3
					YOff = 1
				Case "e"
					XOff = 4
					YOff = 1
				Case "f"
					XOff = 5
					YOff = 1
				Case "g"
					XOff = 6
					YOff = 1
				Case "h"
					XOff = 7
					YOff = 1
				Case "i"
					XOff = 8
					YOff = 1
				Case "j"
					XOff = 9
					YOff = 1
				Case "k"
					XOff = 0
					YOff = 2
				Case "l"
					XOff = 1
					YOff = 2
				Case "m"
					XOff = 2
					YOff = 2
				Case "n"
					XOff = 3
					YOff = 2
				Case "o"
					XOff = 4
					YOff = 2
				Case "p"
					XOff = 5
					YOff = 2
				Case "q"
					XOff = 6
					YOff = 2
				Case "r"
					XOff = 7
					YOff = 2
				Case "s"
					XOff = 8
					YOff = 2
				Case "t"
					XOff = 9
					YOff = 2
				Case "u"
					XOff = 0
					YOff = 3
				Case "v"
					XOff = 1
					YOff = 3
				Case "w"
					XOff = 2
					YOff = 3
				Case "x"
					XOff = 3
					YOff = 3
				Case "y"
					XOff = 4
					YOff = 3
				Case "z"
					XOff = 5
					YOff = 3
				Case "0"
					XOff = 9
					YOff = 0
				Case "1"
					XOff = 0
					YOff = 0
				Case "2"
					XOff = 1
					YOff = 0
				Case "3"
					XOff = 2
					YOff = 0
				Case "4"
					XOff = 3
					YOff = 0
				Case "5"
					XOff = 4
					YOff = 0
				Case "6"
					XOff = 5
					YOff = 0
				Case "7"
					XOff = 6
					YOff = 0
				Case "8"
					XOff = 7
					YOff = 0
				Case "9"
					XOff = 8
					YOff = 0
				Case ","
					XOff = 6
					YOff = 3
				Case "."
					XOff = 7
					YOff = 3
				Case "!"
					XOff = 8
					YOff = 3
				Case "?"
					XOff = 9
					YOff = 3
				Case "@"
					XOff = 1
					YOff = 4
				Case " "
					XOff = 9
					YOff = 4
				Case "/"
					XOff = 0
					YOff = 4
				Case "-"
					XOff = 2
					YOff = 4
				Case ":"
					XOff = 3
					YOff = 4
				Case ";"
					XOff = 4
					YOff = 4
				Case "_"
					XOff = 5
					YOff = 4
				Case "("
					XOff = 6
					YOff = 4
				Case ")"
					XOff = 7
					YOff = 4
				Case "*"
					XOff = 8
					YOff = 4
				Case "+"
					XOff = 9
					YOff = 4
				Default
					XOff = 9
					YOff = 4
					
			End
			
			tmp[i] = New RazChar()
			tmp[i].XOff = XOff
			tmp[i].YOff = YOff
			tmp[i].TextValue = letstring
			If XOff = 9 And YOff = 4
				tmp[i].Visible = False
			EndIf
			
		Next
	
		Local cl:Int = 0
		Local tList:List<RazChar[]> = New List<RazChar[]>
		For Local atmp:RazChar[] = EachIn Lines
			If cl = cLine
				tList.AddLast(tmp)
			Else
				tList.AddLast(atmp)
			EndIf
			cl += 1
		Next 
		
		Lines = tList
		
	End
	
	Method AddLine:Void(tString:String)
	
		' ' Print "Adding String : "+tString
	
		Local tmp:RazChar[] = New RazChar[tString.Length()]
		For Local i:Int = 0 To tString.Length() - 1
			Local let:Int = tString[i]
			Local letstring:String = tString[i..(i+1)].ToLower()
			Local XOff:Int = 0
			Local YOff:Int = 0
			Select letstring.ToLower()
				Case "a"
					XOff = 0
					YOff = 1
				Case "b"
					XOff = 1
					YOff = 1
				Case "c"
					XOff = 2
					YOff = 1
				Case "d"
					XOff = 3
					YOff = 1
				Case "e"
					XOff = 4
					YOff = 1
				Case "f"
					XOff = 5
					YOff = 1
				Case "g"
					XOff = 6
					YOff = 1
				Case "h"
					XOff = 7
					YOff = 1
				Case "i"
					XOff = 8
					YOff = 1
				Case "j"
					XOff = 9
					YOff = 1
				Case "k"
					XOff = 0
					YOff = 2
				Case "l"
					XOff = 1
					YOff = 2
				Case "m"
					XOff = 2
					YOff = 2
				Case "n"
					XOff = 3
					YOff = 2
				Case "o"
					XOff = 4
					YOff = 2
				Case "p"
					XOff = 5
					YOff = 2
				Case "q"
					XOff = 6
					YOff = 2
				Case "r"
					XOff = 7
					YOff = 2
				Case "s"
					XOff = 8
					YOff = 2
				Case "t"
					XOff = 9
					YOff = 2
				Case "u"
					XOff = 0
					YOff = 3
				Case "v"
					XOff = 1
					YOff = 3
				Case "w"
					XOff = 2
					YOff = 3
				Case "x"
					XOff = 3
					YOff = 3
				Case "y"
					XOff = 4
					YOff = 3
				Case "z"
					XOff = 5
					YOff = 3
				Case "0"
					XOff = 9
					YOff = 0
				Case "1"
					XOff = 0
					YOff = 0
				Case "2"
					XOff = 1
					YOff = 0
				Case "3"
					XOff = 2
					YOff = 0
				Case "4"
					XOff = 3
					YOff = 0
				Case "5"
					XOff = 4
					YOff = 0
				Case "6"
					XOff = 5
					YOff = 0
				Case "7"
					XOff = 6
					YOff = 0
				Case "8"
					XOff = 7
					YOff = 0
				Case "9"
					XOff = 8
					YOff = 0
				Case ","
					XOff = 6
					YOff = 3
				Case "."
					XOff = 7
					YOff = 3
				Case "!"
					XOff = 8
					YOff = 3
				Case "?"
					XOff = 9
					YOff = 3
				Case "@"
					XOff = 1
					YOff = 4
				Case " "
					XOff = 9
					YOff = 4
				Case "/"
					XOff = 0
					YOff = 4
				Case "-"
					XOff = 2
					YOff = 4
				Case ":"
					XOff = 3
					YOff = 4
				Case ";"
					XOff = 4
					YOff = 4
				Case "_"
					XOff = 5
					YOff = 4
				Case "("
					XOff = 6
					YOff = 4
				Case ")"
					XOff = 7
					YOff = 4
				Case "*"
					XOff = 8
					YOff = 4
				Case "+"
					XOff = 9
					YOff = 4
				Default
					XOff = 9
					YOff = 4
					
			End
			
			tmp[i] = New RazChar()
			tmp[i].XOff = XOff
			tmp[i].YOff = YOff
			tmp[i].TextValue = letstring
			If XOff = 9 And YOff = 4
				tmp[i].Visible = False
			EndIf
			
		Next
		
		Lines.AddLast(tmp)
		
	End
	
	Method AddMutliLines:Void(tString:String)
		
		Local tmp:String[] = tString.Split("~r")
		For Local i:Int = 0 To tmp.Length() - 1
			AddLine(tmp[i])
		Next
	
	End
	Method SetPos:Void(tX:Int,tY:Int)
		X = tX
		Y = tY
	End
	
	
	Method Draw:Void()
		Draw(X,Y)
	End
	
	Method Draw:Void(tX:Int,tY:Int)
	
		Local drawn:Int = 0
	
		Local cY:Int = tY
		Local cX:Int = tX
		For Local tLine:RazChar[] = EachIn Lines
			For Local i:Int = 0 To tLine.Length() - 1
				If tLine[i].Visible = True
					If RectOverRect(cX, cY, CharacterWidth, CharacterHeight, 0, 0, LDApp.ScreenWidth, LDApp.ScreenHeight) = True
						drawn += 1
						DrawImageRect(TextSheet, cX, cY, CharacterOriginX + (tLine[i].XOff * CharacterWidth), CharacterOriginY + (tLine[i].YOff * CharacterHeight), CharacterWidth, CharacterHeight)
					End
				End
				cX += CharacterWidth + HorizontalSpacing
				
			Next
			cY += CharacterHeight + VerticalSpacing
			cX = tX
		Next
		
	End

	Method GetString:String()
		Return OriginalString
	End
	
End

Class RazChar

	Field Visible:Bool = True
	
	Field XOff:Int
	Field YOff:Int
	Field TextValue:String

End