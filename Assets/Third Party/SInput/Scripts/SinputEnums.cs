﻿//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

namespace SinputSystems{
	public enum ButtonAction{
		HELD, //held down
		DOWN, //pressed this frame
		UP, //released this frame
		NOTHING, //for no action

		REPEATING //for repeating presses, it can be a type of check but NOT a state a button can itself be in
	}

	public enum MouseInputType{
		None,
		MouseMoveLeft,
		MouseMoveRight,
		MouseMoveUp,
		MouseMoveDown,
		MouseHorizontal,
		MouseVertical,
		MouseScrollUp,
		MouseScrollDown,
		MouseScroll,
		MousePositionX,
		MousePositionY,
		Mouse0,
		Mouse1,
		Mouse2,
		Mouse3,
		Mouse4,
		Mouse5,
		Mouse6
	}
	public enum KeyboardInputType{
		None,
		UpArrow,
		DownArrow,
		RightArrow,
		LeftArrow,
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
		K,
		L,
		M,
		N,
		O,
		P,
		Q,
		R,
		S,
		T,
		U,
		V,
		W,
		X,
		Y,
		Z,
		RightShift,
		LeftShift,
		RightControl,
		LeftControl,
		RightAlt,
		LeftAlt,
		LeftCommand,
		LeftApple,
		LeftWindows,
		RightCommand,
		RightApple,
		RightWindows,
		AltGr,
		Backspace,
		Delete,
		Tab,
		Clear,
		Return,
		Pause,
		Escape,
		Space,
		Keypad0,
		Keypad1,
		Keypad2,
		Keypad3,
		Keypad4,
		Keypad5,
		Keypad6,
		Keypad7,
		Keypad8,
		Keypad9,
		KeypadPeriod,
		KeypadDivide,
		KeypadMultiply,
		KeypadMinus,
		KeypadPlus,
		KeypadEnter,
		KeypadEquals,
		Insert,
		Home,
		End,
		PageUp,
		PageDown,
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		F10,
		F11,
		F12,
		F13,
		F14,
		F15,
		Alpha0,
		Alpha1,
		Alpha2,
		Alpha3,
		Alpha4,
		Alpha5,
		Alpha6,
		Alpha7,
		Alpha8,
		Alpha9,
		Exclaim,
		DoubleQuote,
		Hash,
		Dollar,
		Ampersand,
		Quote,
		LeftParen,
		RightParen,
		Asterisk,
		Plus,
		Comma,
		Minus,
		Period,
		Slash,
		Colon,
		Semicolon,
		Less,
		Equals,
		Greater,
		Question,
		At,
		LeftBracket,
		Backslash,
		RightBracket,
		Caret,
		Underscore,
		BackQuote,
		Numlock,
		CapsLock,
		ScrollLock,
		Help,
		Print,
		SysReq,
		Break,
		Menu,
	}

	public enum UnityGamepadKeyCode{

		JoystickButton0 = 330,
		JoystickButton1 = 331,
		JoystickButton2 = 332,
		JoystickButton3 = 333,
		JoystickButton4 = 334,
		JoystickButton5 = 335,
		JoystickButton6 = 336,
		JoystickButton7 = 337,
		JoystickButton8 = 338,
		JoystickButton9 = 339,
		JoystickButton10 = 340,
		JoystickButton11 = 341,
		JoystickButton12 = 342,
		JoystickButton13 = 343,
		JoystickButton14 = 344,
		JoystickButton15 = 345,
		JoystickButton16 = 346,
		JoystickButton17 = 347,
		JoystickButton18 = 348,
		JoystickButton19 = 349,
		Joystick1Button0 = 350,
		Joystick1Button1 = 351,
		Joystick1Button2 = 352,
		Joystick1Button3 = 353,
		Joystick1Button4 = 354,
		Joystick1Button5 = 355,
		Joystick1Button6 = 356,
		Joystick1Button7 = 357,
		Joystick1Button8 = 358,
		Joystick1Button9 = 359,
		Joystick1Button10 = 360,
		Joystick1Button11 = 361,
		Joystick1Button12 = 362,
		Joystick1Button13 = 363,
		Joystick1Button14 = 364,
		Joystick1Button15 = 365,
		Joystick1Button16 = 366,
		Joystick1Button17 = 367,
		Joystick1Button18 = 368,
		Joystick1Button19 = 369,
		Joystick2Button0 = 370,
		Joystick2Button1 = 371,
		Joystick2Button2 = 372,
		Joystick2Button3 = 373,
		Joystick2Button4 = 374,
		Joystick2Button5 = 375,
		Joystick2Button6 = 376,
		Joystick2Button7 = 377,
		Joystick2Button8 = 378,
		Joystick2Button9 = 379,
		Joystick2Button10 = 380,
		Joystick2Button11 = 381,
		Joystick2Button12 = 382,
		Joystick2Button13 = 383,
		Joystick2Button14 = 384,
		Joystick2Button15 = 385,
		Joystick2Button16 = 386,
		Joystick2Button17 = 387,
		Joystick2Button18 = 388,
		Joystick2Button19 = 389,
		Joystick3Button0 = 390,
		Joystick3Button1 = 391,
		Joystick3Button2 = 392,
		Joystick3Button3 = 393,
		Joystick3Button4 = 394,
		Joystick3Button5 = 395,
		Joystick3Button6 = 396,
		Joystick3Button7 = 397,
		Joystick3Button8 = 398,
		Joystick3Button9 = 399,
		Joystick3Button10 = 400,
		Joystick3Button11 = 401,
		Joystick3Button12 = 402,
		Joystick3Button13 = 403,
		Joystick3Button14 = 404,
		Joystick3Button15 = 405,
		Joystick3Button16 = 406,
		Joystick3Button17 = 407,
		Joystick3Button18 = 408,
		Joystick3Button19 = 409,
		Joystick4Button0 = 410,
		Joystick4Button1 = 411,
		Joystick4Button2 = 412,
		Joystick4Button3 = 413,
		Joystick4Button4 = 414,
		Joystick4Button5 = 415,
		Joystick4Button6 = 416,
		Joystick4Button7 = 417,
		Joystick4Button8 = 418,
		Joystick4Button9 = 419,
		Joystick4Button10 = 420,
		Joystick4Button11 = 421,
		Joystick4Button12 = 422,
		Joystick4Button13 = 423,
		Joystick4Button14 = 424,
		Joystick4Button15 = 425,
		Joystick4Button16 = 426,
		Joystick4Button17 = 427,
		Joystick4Button18 = 428,
		Joystick4Button19 = 429,
		Joystick5Button0 = 430,
		Joystick5Button1 = 431,
		Joystick5Button2 = 432,
		Joystick5Button3 = 433,
		Joystick5Button4 = 434,
		Joystick5Button5 = 435,
		Joystick5Button6 = 436,
		Joystick5Button7 = 437,
		Joystick5Button8 = 438,
		Joystick5Button9 = 439,
		Joystick5Button10 = 440,
		Joystick5Button11 = 441,
		Joystick5Button12 = 442,
		Joystick5Button13 = 443,
		Joystick5Button14 = 444,
		Joystick5Button15 = 445,
		Joystick5Button16 = 446,
		Joystick5Button17 = 447,
		Joystick5Button18 = 448,
		Joystick5Button19 = 449,
		Joystick6Button0 = 450,
		Joystick6Button1 = 451,
		Joystick6Button2 = 452,
		Joystick6Button3 = 453,
		Joystick6Button4 = 454,
		Joystick6Button5 = 455,
		Joystick6Button6 = 456,
		Joystick6Button7 = 457,
		Joystick6Button8 = 458,
		Joystick6Button9 = 459,
		Joystick6Button10 = 460,
		Joystick6Button11 = 461,
		Joystick6Button12 = 462,
		Joystick6Button13 = 463,
		Joystick6Button14 = 464,
		Joystick6Button15 = 465,
		Joystick6Button16 = 466,
		Joystick6Button17 = 467,
		Joystick6Button18 = 468,
		Joystick6Button19 = 469,
		Joystick7Button0 = 470,
		Joystick7Button1 = 471,
		Joystick7Button2 = 472,
		Joystick7Button3 = 473,
		Joystick7Button4 = 474,
		Joystick7Button5 = 475,
		Joystick7Button6 = 476,
		Joystick7Button7 = 477,
		Joystick7Button8 = 478,
		Joystick7Button9 = 479,
		Joystick7Button10 = 480,
		Joystick7Button11 = 481,
		Joystick7Button12 = 482,
		Joystick7Button13 = 483,
		Joystick7Button14 = 484,
		Joystick7Button15 = 485,
		Joystick7Button16 = 486,
		Joystick7Button17 = 487,
		Joystick7Button18 = 488,
		Joystick7Button19 = 489,
		Joystick8Button0 = 490,
		Joystick8Button1 = 491,
		Joystick8Button2 = 492,
		Joystick8Button3 = 493,
		Joystick8Button4 = 494,
		Joystick8Button5 = 495,
		Joystick8Button6 = 496,
		Joystick8Button7 = 497,
		Joystick8Button8 = 498,
		Joystick8Button9 = 499,
		Joystick8Button10 = 500,
		Joystick8Button11 = 501,
		Joystick8Button12 = 502,
		Joystick8Button13 = 503,
		Joystick8Button14 = 504,
		Joystick8Button15 = 505,
		Joystick8Button16 = 506,
		Joystick8Button17 = 507,
		Joystick8Button18 = 508,
		Joystick8Button19 = 509,
		Joystick9Button0 = 510,
		Joystick9Button1 = 511,
		Joystick9Button2 = 512,
		Joystick9Button3 = 513,
		Joystick9Button4 = 514,
		Joystick9Button5 = 515,
		Joystick9Button6 = 516,
		Joystick9Button7 = 517,
		Joystick9Button8 = 518,
		Joystick9Button9 = 519,
		Joystick9Button10 = 520,
		Joystick9Button11 = 521,
		Joystick9Button12 = 522,
		Joystick9Button13 = 523,
		Joystick9Button14 = 524,
		Joystick9Button15 = 525,
		Joystick9Button16 = 526,
		Joystick9Button17 = 527,
		Joystick9Button18 = 528,
		Joystick9Button19 = 529,
		Joystick10Button0 = 530,
		Joystick10Button1 = 531,
		Joystick10Button2 = 532,
		Joystick10Button3 = 533,
		Joystick10Button4 = 534,
		Joystick10Button5 = 535,
		Joystick10Button6 = 536,
		Joystick10Button7 = 537,
		Joystick10Button8 = 538,
		Joystick10Button9 = 539,
		Joystick10Button10 = 540,
		Joystick10Button11 = 541,
		Joystick10Button12 = 542,
		Joystick10Button13 = 543,
		Joystick10Button14 = 544,
		Joystick10Button15 = 545,
		Joystick10Button16 = 546,
		Joystick10Button17 = 547,
		Joystick10Button18 = 548,
		Joystick10Button19 = 549,
		Joystick11Button0 = 550,
		Joystick11Button1 = 551,
		Joystick11Button2 = 552,
		Joystick11Button3 = 553,
		Joystick11Button4 = 554,
		Joystick11Button5 = 555,
		Joystick11Button6 = 556,
		Joystick11Button7 = 557,
		Joystick11Button8 = 558,
		Joystick11Button9 = 559,
		Joystick11Button10 = 560,
		Joystick11Button11 = 561,
		Joystick11Button12 = 562,
		Joystick11Button13 = 563,
		Joystick11Button14 = 564,
		Joystick11Button15 = 565,
		Joystick11Button16 = 566,
		Joystick11Button17 = 567,
		Joystick11Button18 = 568,
		Joystick11Button19 = 569,
	}

	public enum InputDeviceType{
		Keyboard,
		GamepadButton,
		GamepadAxis,
		Mouse,
		Virtual
	}

	public enum OSFamily{
		Other,
		MacOSX,
		Windows,
		Linux,
		Android,
		IOS,
		PS4,
		PSVita,
		XboxOne,
		Switch
	}

	public enum InputDeviceSlot{
		gamepad1=1,
		gamepad2=2,
		gamepad3=3,
		gamepad4=4,
		gamepad5=5,
		gamepad6=6,
		gamepad7=7,
		gamepad8=8,
		gamepad9=9,
		gamepad10=10,
		gamepad11=11,
		gamepad12=12,
		gamepad13=13,
		gamepad14=14,
		gamepad15=15,
		gamepad16=16,
		keyboardAndMouse=17,
		keyboard=18,
		mouse=19,
		virtual1=20,
		any=0,
	}

	public enum CommonGamepadInputs{
		NOBUTTON=0,
		A=1,
		B=2,
		X=3,
		Y=4,
		LB=5,
		RB=6,
		LT=7,
		RT=8,
		L3=9,
		R3=10,
		DPAD_LEFT=11,
		DPAD_RIGHT=12,
		DPAD_UP=13,
		DPAD_DOWN=14,
		LSTICK_LEFT=15,
		LSTICK_RIGHT=16,
		LSTICK_UP=17,
		LSTICK_DOWN=18,
		RSTICK_LEFT=19,
		RSTICK_RIGHT=20,
		RSTICK_UP=21,
		RSTICK_DOWN=22,
		START=23,
		BACK=24,//AKA select/menu/whatever
		HOME=25,//AKA system

		LSTICK_X=26,
		LSTICK_Y=27,
		RSTICK_X=28,
		RSTICK_Y=29,
		DPAD_X=30,
		DPAD_Y=31,
	}
}
