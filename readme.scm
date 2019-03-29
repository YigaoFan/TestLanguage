(define Word
	(
	(Once Prepare)
	(NewBlank)
	[
		(CheckResume)

		(
			(OpenFiletab)
		 	[
		 		(ViewInfo)
		 		(
		 			(CheckOption)
		 			(CheckedOption)
		 			[
		 				PressOK
		 				PressCancel
		 			]
		 		)
		)
	]
	(Quit)
))
; [] 里的平级步骤是作为分支存在的，[] 对外是作为一个步骤存在的
; () 里的步骤作为顺序步骤存在，对外作为一个步骤存在
; 选择结构需要指定选择，否则默认选择第一项
; 其实进入深层步骤后，到最后的 Quit 一般是需要一些返回操作步骤，但一般这些步骤可以不写，默认到指定的步骤终止(这条可能不完善，说的是测试程序的返回处理)，可配置成以 Quit 终止
; Prepare 和 Quit 名的步骤为特殊步骤，Prepare 只在第一次操作运行，之后的测试不再运行，Quit 为可配置的终止步骤
; define 可以定义一个结构(?)