// mobileBridge.jslib

mergeInto(LibraryManager.library, {

   DetectDevice: function DetectDevice() {

     var  device;

     if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) 
     {
       device = "mobile";
     }
     else
    {
       device = "desktop";

    }


     if (window.unityInstance != null) {
          //calls function of CanvasManager and sends the result to the unity application 
          window.unityInstance.SendMessage('NetworkManager', 'OnDetectDevice', device);
        } //END_IF
  
  }
  
 
});