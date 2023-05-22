# Pushdemoformaui
Export xamarin with hubs sample to maui (https://github.com/xamcat/mobcat-samples/tree/master/notification_hub_backend_service/src/xamarin)

* Details for project with backend: https://learn.microsoft.com/en-us/azure/developer/mobile-apps/notification-hubs-backend-service-xamarin-forms

Main changes:

* Use net 7 for nuget packages compatibility. This build works explicitly with net 7 and listed versions of (xamarin.googleplayservices) and (xamarin.firebase) packages.

* For build action of google-servicesjson, install and import xamarin.googleplayservices.basement package to csproj file just before closing tag for project element. For example: 

    `<Import Project="C:\Users\Administrator\.nuget\packages\xamarin.googleplayservices.basement\118.1.0\build\MonoAndroid12.0\Xamarin.GooglePlayServices.Basement.targets" Condition="Exists('C:\Users\Administrator\.nuget\packages\xamarin.googleplayservices.basement\118.1.0\build\MonoAndroid12.0\Xamarin.GooglePlayServices.Basement.targets')" />`

  More on import element: https://learn.microsoft.com/en-us/visualstudio/msbuild/import-element-msbuild?view=vs-2022

  Reference https://stackoverflow.com/questions/42677766/cant-set-build-action-to-googleservicejson

* Move service regesteration from bootstrap to app.xaml.cs constructor if possible.

* After each build, an error about auto generated AndroidManifest.xml. In (obj\Debug\net7.0-android\AndroidManifest.xml), add attribut (android:exported="true") to element service with (android:name) attribute value ending with PushNotificationFirebaseMessagingService and rebuild. Sometimes, after adding new xaml pages, problem persists, even after rebuild. Reason is old built assets are there, VS clean also does not solve it. Solutions is to redownload code from remote and build on clean state.



