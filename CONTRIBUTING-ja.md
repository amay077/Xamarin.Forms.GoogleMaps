# How to contribute

[English contribution guideline is here！](CONTRIBUTING.md)

我々は皆さんの Xamarin.Forms.GoogleMaps(以下、XF.GoogleMaps) へ貢献を歓迎します。
我々は、皆さんによる貢献を出来るだけ簡単にできるようにしたいと考えています。
その為に、貢献しようとする人は以下の方針に従うよう、お願いします。

## Getting Started

* XF.GoogleMaps に貢献する為には [GitHub アカウント](https://github.com/signup/free) が必要です。
* 問題や機能要望には Issue を作ってください（まだ Issue が作られていない場合）。
  * Issue や Pull request を送るのに、事前の連絡は必要ありません。
  * バグを issue で報告する場合、バグを再現する為の説明、エラーの情報、環境を書いてください。
  * Issue のタイトルと本文はできるだけ英語で書いてください（不可能な場合は日本語でも OK です）。
  * [Issue テンプレート](ISSUE_TEMPLATE.md) が用意されているので、必要な箇所を切り取って使用してください。
* GitHub でリポジトリの fork を作ってください。
* ``git clone https://github.com/[your_account]/Xamarin.Forms.GoogleMaps.git`` でローカルにクローンしたあと、 ``git config user.name yourname`` と ``git config user.email your@ema.il`` を設定してください。特に、``user.email`` は必ず GitHub のアカウントで使用している e-mail アドレスを設定してください。

## Making Changes

* コードやドキュメントを XF.GoogleMaps に貢献するにはベースとなるブランチから、トピック・ブランチを作ってください。
  * 通常、これ(ベースとなるブランチ)は master ブランチです。
  * master ブランチから、トピック・ブランチを作るには： `git branch
    issue_999 master` してから `git
    checkout issue_999`で新しいブランチに切り替えます。master ブランチ上で作業するのを避けてください。
    間違って、master ブランチ上で作業しないように、[pre-commit hook で master への commit を禁止した](http://blog.n-z.jp/blog/2014-02-07-pre-commit-hook.html) のような仕掛け
    をいれるのも良いでしょう。
* commit は合理的(ロジック単位)に分けてください。また目的と関係のないコードの変更は含めないでください(コードフォーマットの変更、不要コードの削除など)。
* commit メッセージが正しいフォーマットにあることを確認してください。commit メッセージはできるだけ英語でお願いします。

````
必須なコミット情報はここに（できるだけ英語で）。

上にある最初の列はパッチ/コミットの概要を説明します。
この本文はパッチが無い状態のプログラムの行動、なんでこの行動は問題なのか、どうやってパッチが問題を解決するのかを説明します。
````

* 変更の為にテストが必要ならそのテストが追加されているよう確認してください。

## Coding Style

我々は [.NET Foundation](https://github.com/dotnet/corefx/blob/master/Documentation/coding-guidelines/coding-style.md) のコーディングスタイルを使用します。ただし、1点違いがあります。

> We use Allman style braces, where each brace begins on a new line. ~~A single line statement block can go without braces~~

我々は、**常に** 中括弧を使用します、1行の ``if/for/while/etc`` でも ``{ }`` を省略しないでください。

**例:**

```csharp
// 👎🏽 DO NOT USE
if (source == null) 
    throw new ArgumentNullException("source");

// 👍🏽 GOOD
if (source == null)
{
    throw new ArgumentNullException("source");
}
```

コード中には、コメントも含めて日本語は使わないでください。

## Submitting Changes

* 自分の fork で、変更をトピック・ブランチに push してください。
* XF.GoogleMaps のリポジトリに pull request を投稿してください。

pull request は、以下のように作成してください。

* タイトルは変更の要約を分かりやすく書いてください。
* その変更が確認できるコードの場所を明記してください。通常これは [サンプルプログラム](https://github.com/amay077/Xamarin.Forms.GoogleMaps/tree/master/XFGoogleMapSample) に追加することを推奨します。
* 本文には、関連する issue の番号を本文に含めてください。( ref #199 など)
* 本文には、その変更が確認できるコードの場所を明記してください。通常これは [サンプルプログラム](https://github.com/amay077/Xamarin.Forms.GoogleMaps/tree/master/XFGoogleMapSample) に追加することを推奨します。
* まだ作業中である場合、タイトルの先頭に [WIP] を付けてください。マージ可能になったら、[WIP] を除去し、コメントでお知らせください。
  * 作業中に master ブランチが変更された場合は、コンフリクトを解消してから [WIP] を除去してください。
  * その作業を予約する意味で、まず [WIP] の付いた pull request を投稿することを許可します。ただし、長い間活動が見られない場合は、クローズされる場合があります。

# Thanks

このドキュメントは、[MMP/CONTRIBUTING.md · sn0w75/MMP](https://github.com/sn0w75/MMP/blob/master/CONTRIBUTING.md) をベースに作成しました。
