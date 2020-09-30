# ActiveLine.Server
 
## Overview

ActiveLineのサーバー機能を実現する。

## Description
 
機能カテゴリとして以下の区分になる。

### Core

ActiveLineのCore機能、配信機能と収集機能に関連する機能を持つ。

### Operation

ActiveLineの顧客システムの保守に関連する機能を持つ。

### Utility

ActiveLineの監視に関連する機能を持つ。

## Requirement

- .Net Core 2.1
  - Function Apps v2.0が推奨されているため。
- Microsoft.NET.Sdk.Functions 11.0.2
- Microsoft.WindowsAzure.Storage
  - WebJobsでの使用が想定されているため、WebJobsが内部で参照している"WindowsAzure.Storage"を使用する。(WebJobsでMicrosoft.Azure.Storageを使用するとバージョンにより実行時エラーになるらしい([\.NET Standard や \.NET Core におけるAzureのStorage関連nugetパッケージの選択 \- tech\.guitarrapc\.cóm](https://tech.guitarrapc.com/entry/2019/01/24/041510)))
- Moq
  - 単体テスト用
- Microsoft.EntityFrameworkCore 2.1.*
  - .Net Coreのバージョンに合わせるようにする
    - 直下のパッケージ(Microsoft.EntityFrameworkCore.XXX)も同様にバージョンを揃えた方が動作しやすい
  - 本パッケージがこれより新しいバージョンの場合、Azure Functions実行時に参照エラーが発生して起動できない。(v3.1.1で現象を確認)
- Microsoft.EntityFrameworkCore.XXX.YYY
  - 個別インストールの場合は、2.1.*のバージョンでなくてよい
- Microsoft.Azure.Functions.Extensions -Version 1.0.0
  - Azure Functions StartupでDIを行うために必要。
  - バージョンについてはその時点での最新です。
- Microsoft.NET.Sdk.Functions -Version 1.0.29
  - Azure Functions StartupでDIを行うために必要。
  -  ~~バージョンについてはその時点での最新です。~~
  - 異なるバージョンが混在した場合（ `1.0.29` および `1.0.31` ）にMSBuildエラー（ `MSB4063` および `MSB4064` ）が発生したため、バージョンを固定する。
      -  ビルドに問題がなかった `1.0.29` に統一する（2020/03/26時点）。
- AzureFunctions.Extensions.Swashbuckle v1.4.4
  - WebAPIの仕様をSwaggerで表現しているが、Azure Functions のWebAPIコードから直接Swaggerにはできない。その間を埋めるためのサードパーティパッケージ。
  - バージョンについてはその時点での最新です。
- Microsoft.Azure.Devices v1.18.2
  - IotHubsのSDK。
  - この時点での最新はv1.18.4だったが、本バージョンは「Newtonsoft.Json v12.0.3以上」に依存している。この依存は「AzureFunctions.Extensions.Swashbuckle v1.4.4」の依存ツリーの中で依存している「Newtonsoft.Json v11.0.2」と競合しているため、「Newtonsoft.Json v10.0.3以上」依存のv1.18.2を使用している。
- Microsoft.Azure.Devices.Provisioning.Service -Version 1.5.0
  - IoTHubDPSの、サービス側のSDK。
  - 同.DeviceというSDKで登録が可能。
  - この時点での最新はv1.5.2だが、このバージョンは「Newtonsoft.Json v12.0.3」に依存するため、「AzureFunctions.Extensions.Swashbuckle v1.4.4」の依存ツリーの中で依存している「Newtonsoft.Json v11.0.2」と競合する。そのため、依存が「Newtonsoft.Json v10.0.3」の「v1.5.0」を使用している。
- Install-Package Microsoft.ApplicationInsights -Version 2.13.1
  - PowerApps側のリクエストとWebAPIの処理を紐づける方法がないため、ITelemetryInitializerを実装してリクエストヘッダ「x-ms-*」を出力する。これは、PowerApps側から自動で付与されるヘッダ情報である。
  - バージョンはこの時点での最新。

## 開発環境

- Microsoft Visual Studio Professional 2017  
- Windows 10

## プロジェクト構成

```
Rms.Server.Core.sln
+ Abstraction
+ DBAccesser
+ Utils
+ Azure.Functions.StartUp
+ Azure.Functions.Utility
+ Azure.Functions.WebApi
+ Azure.Functions.BlobCleaner
+ Azure.Functions.Deliveryer
+ Azure.Functions.DbCleaner
+ Azure.Functions.Dispatcher
+ Service
+ Utility
```

## Usage
 
1. Usage  
2. Usage  
3. Usage
 
## Installation
 
```
$ git clone https://github.com/TomoakiTANAKA/awesome-tool
$ cd awesome-tool
$ sh setup.sh
$ ~do anything~
```

## 静的テスト

StyleCopを使用する。
必要に応じて不要、非効率なルールは抑制すること。
Testクラス、TestHelperについては全ルールを抑制した設定を各プロジェクトフォルダに配置する。
 
## UnitTest

1. デバッグビルド  
2. ソリューションフォルダにある「UnitTest.bat」を実行  
3. UnitTestResultsフォルダにあるtrxファイルをVisual Studioで開いてテスト結果を確認
4. UnitTestResults\coverage\index.htmを開いてカバレッジを確認
 
## Deploy
 
1. deploy  
2. deploy  
3. deploy
 
## Anything Else
 
AnythingAnythingAnything
AnythingAnythingAnything
AnythingAnythingAnything

## Utilityのビルド手順

### VisualStudio2017インストール環境
1．Rms.Server.Utility内のLibBuild.batをたたく
　(UtilityのバッチをたたくとCoreとOperationもビルドするようになっている)
2．Rms.Server.Utilityをビルド

### VisualStudio2017非インストール環境

1．以下フォルダー内にMSBuild.exeがあるか確認
　C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\150\Bin\MSBuild.exe
2．Rms.Server.Utility内のLibBuild.batをたたく
3．Rms.Server.Utilityをビルド

### 上記操作を行い、それでもビルド出来なかったときは手動でビルドを行う

1．Rms.Server.Coreをビルドする
2．下記フォルダー内の、下記dllファイルをコピーしRms.Server.Operation\libにペースト
　Rms.Server.Core\Abstraction\bin\Debug\netcoreapp2.1
　　Rms.Server.Core.Abstraction.dll
　　Rms.Server.Core.Utility.dll
3．Rms.Server.Operationをビルド
4．下記フォルダー内の、下記dllファイルをコピーしRms.Server.Utility\libにペースト
　Rms.Server.Operation\Abstraction\bin\Debug\netcoreapp2.1
　　Rms.Server.Core.Abstraction.dll
　　Rms.Server.Core.Utility.dll
　　Rms.Server.Operation.Utility.dll
5．Rms.Server.Utilityをビルド



### トラブルシューティング

* AzureFunctionsをVS上でデバッグ実行したらエラーがでた

```
  The listener for function 'BlobIndexer' was unable to start.
[2020/04/27 0:54:24] The listener for function 'BlobIndexer' was unable to start. Microsoft.WindowsAzure.Storage: 対象のコンピューターによって拒否されたため、接続できませんでした。. System.Net.Http: 対象のコンピューターによって拒否されたた め、接続できませんでした。. System.Private.CoreLib: 対象のコンピューターによって拒否されたため、接続できませんでした。.
```

→StorageEmulatorを手動で起動する。