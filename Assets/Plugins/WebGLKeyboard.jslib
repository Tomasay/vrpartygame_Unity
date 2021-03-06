mergeInto(LibraryManager.library, {

	OnKeyboardInput: function ()
	{
		console.log("OnKeyboardInput");
		unityInstance.SendMessage('KeyboardController', 'UpdateText', document.getElementById("dummyInput").value.toString());
	}, 
	CreateDummyInput: function ()
	{
		var divElement = document.getElementById("main-container");
        var inputElement = document.createElement("input");
        inputElement.type = "text";
        inputElement.id = "dummyInput";
        inputElement.style = "font-size: 16px; position:absolute; bottom:25%;";
        divElement.appendChild(inputElement);
		inputElement.oninput = function() {console.log("ON INPUT"); window.unityInstance.SendMessage('KeyboardController', 'UpdateText', document.getElementById("dummyInput").value.toString());}
	},
	OpenInputKeyboard: function () 
	{
		//document.getElementById("dummyInput").focus();
		//fakeInputFocus();
		//document.getElementById("fakeInputButton").click();
		console.log("OpenInputKeyboard");
	},
	CloseInputKeyboard: function ()
	{
		document.getElementById("dummyInput").blur();
	},
	UpdateInputFieldText: function (txt)
	{
		document.getElementById("dummyInput").value = Pointer_stringify(txt);
	},
	SetPointerDownOnButton: function (isDown)
	{
		window.isPointerDownOnButton = isDown;
	},
	ReloadPage: function ()
	{
		window.location.reload();
	}
});