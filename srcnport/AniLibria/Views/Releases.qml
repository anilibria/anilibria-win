import QtQuick 2.12
import QtQuick.Controls 2.12
import QtQuick.Layouts 1.3
import QtWebView 1.1
import QtQuick.Controls.Styles 1.4
import QtQuick.Dialogs 1.2
import QtGraphicalEffects 1.0
import Anilibria.Services 1.0
import "../Controls"

Page {
    id: page
    property bool selectMode
    property var selectedReleases: []
    property var displayedReleases: []
    property var favoriteReleases: []
    property int pageIndex: 1
    property bool isBusy: false
    property var openedRelease: null
    property bool runRefreshFavorties: false
    property bool synchronizeEnabled: false
    property int selectedSection: 0
    property var sections: ["Все релизы", "Избранное", "Новые релизы", "Новые серии", "Обновленные торренты"]

    signal navigateFrom()
    signal watchRelease(int releaseId, string videos)
    signal refreshReleases()
    signal refreshFavorites(var userFavorites)

    onWidthChanged: {
        const columnCount = parseInt(page.width / 520);
        itemGrid.columns = columnCount < 1 ? 1 : columnCount;
    }

    onRefreshReleases: {
        refreshAllReleases();
    }

    onRefreshFavorites: {
        favoriteReleases = userFavorites;

        if (page.runRefreshFavorties && page.selectedReleases.length) {
            page.runRefreshFavorties = false;
            page.selectedReleases = [];
        }
    }

    background: Rectangle {
        color: "#D3D3D3"
    }

    anchors.fill: parent

    Rectangle {
        id: mask
        width: 180
        height: 260
        radius: 10
        visible: false
    }

    Rectangle {
        id: cardMask
        width: 180
        height: 260
        radius: 6
        visible: false
    }

    RowLayout {
        id: panelContainer
        anchors.fill: parent
        spacing: 0
        Rectangle {
            color: "#9e2323"
            width: 40
            Layout.fillHeight: true
            Column {
                IconButton {
                    height: 45
                    width: 40
                    iconColor: "white"
                    iconPath: "../Assets/Icons/menu.svg"
                    iconWidth: 29
                    iconHeight: 29
                    onButtonPressed: {
                        drawer.open();
                    }
                }
                IconButton {
                    id: favoriteMenuButton
                    height: 45
                    width: 40
                    iconColor: "white"
                    iconPath: "../Assets/Icons/favorite.svg"
                    iconWidth: 29
                    iconHeight: 29
                    onButtonPressed: {
                        if (!page.selectedReleases.length) {
                            favoritePopupHeader.text = "Избранное не доступно";
                            favoritePopupMessage.text = "Выберите релизы в списке путем изменения переключателя выше списка на множественный режим и нажатием ЛКМ на интересующих релизах в списке. Выбранные релизы подсвечиваются красной рамкой.";
                            messagePopup.open();
                            return;
                        }

                        if (!window.userModel.login) {
                            favoritePopupHeader.text = "Избранное не доступно";
                            favoritePopupMessage.text = "Чтобы добавлять в избранное нужно вначале авторизоваться. Для этого перейдите на страницу Войти в меню и войдите под данными своего аккаунта. Если вы не зарегистрированы то необходимо сделать это на сайте, ссылка на сайт будет на странице Войти.";
                            messagePopup.open();
                            return;
                        }

                        favoriteMenu.open();
                    }

                    Menu {
                        id: favoriteMenu
                        y: favoriteMenuButton.height

                        MenuItem {
                            font.pixelSize: 14
                            text: "Добавить в избранное"
                            onPressed: {
                                page.runRefreshFavorties = true;
                                synchronizationService.addUserFavorites(applicationSettings.userToken, page.selectedReleases.join(','));
                            }
                        }
                        MenuItem {
                            font.pixelSize: 14
                            text: "Удалить из избранного"
                            onPressed: {
                                page.runRefreshFavorties = true;
                                synchronizationService.removeUserFavorites(applicationSettings.userToken, page.selectedReleases.join(','));
                            }
                        }
                    }

                    Popup {
                        id: messagePopup
                        x: window.width / 2 - 225
                        y: window.height / 2 - 100
                        width: 450
                        height: 150
                        modal: true
                        focus: true
                        closePolicy: Popup.CloseOnEscape | Popup.CloseOnPressOutsideParent

                        Column {
                            width: parent.width
                            spacing: 10
                            Text {
                                id: favoritePopupHeader
                                width: messagePopup.width - 20
                                font.pixelSize: 14
                                font.bold: true
                                elide: Text.ElideRight
                            }

                            Rectangle {
                                width: messagePopup.width - 20
                                height: messagePopup.height - 50
                                Text {
                                    id: favoritePopupMessage
                                    anchors.verticalCenter: parent.verticalCenter
                                    width: parent.width
                                    font.pixelSize: 12
                                    wrapMode: Text.WordWrap
                                }
                            }
                        }
                    }
                }
            }
        }

        ColumnLayout {
            Layout.fillHeight: true
            Layout.fillWidth: true
            spacing: 2

            Rectangle {
                Layout.fillWidth: true
                Layout.preferredHeight: 45
                height: 45
                color: "#808080"
                Row {
                    Switch {
                        id: multupleMode
                        onCheckedChanged: {
                            page.selectMode = checked;
                            if (!checked) {
                                page.selectedReleases = [];
                            } else {
                                page.openedRelease = null;
                            }
                        }
                    }
                    Text {
                        anchors.verticalCenter: parent.verticalCenter
                        visible: page.synchronizeEnabled
                        font.pointSize: 12
                        text: "Выполняется синхронизация..."
                    }
                }
                Text {
                    text: page.sections[page.selectedSection]
                    anchors.verticalCenter: parent.verticalCenter
                    anchors.right: parent.right
                    anchors.rightMargin: 8
                    font.pointSize: 12
                }
            }

            Rectangle {
                id: filtersContainer
                Layout.preferredWidth: 300
                Layout.alignment: Qt.AlignHCenter
                Layout.preferredHeight: 36
                color: "transparent"

                Row {
                    width: filtersContainer.width
                    spacing: 8
                    RoundedTextBox {
                        id: filterByTitle
                        width: 200
                        height: 30
                        fontSize: 12
                        placeholder: "Введите название релиза"
                        onCompleteEditing: {
                            refreshAllReleases();
                        }
                    }
                    IconButton {
                        height: 30
                        width: 30
                        iconColor: "black"
                        hoverColor: "white"
                        iconPath: "../Assets/Icons/allreleases.svg"
                        iconWidth: 24
                        iconHeight: 24
                        onButtonPressed: {
                            changeSection(0);
                        }
                    }
                    IconButton {
                        height: 30
                        width: 30
                        iconColor: "black"
                        hoverColor: "white"
                        iconPath: "../Assets/Icons/favorite.svg"
                        iconWidth: 24
                        iconHeight: 24
                        onButtonPressed: {
                            changeSection(1);
                        }
                    }
                    IconButton {
                        id: notificationMenuButton
                        height: 30
                        width: 30
                        iconColor: "black"
                        hoverColor: "white"
                        iconPath: "../Assets/Icons/notification.svg"
                        iconWidth: 24
                        iconHeight: 24
                        onButtonPressed: {
                            notificationsMenuSections.open();
                        }

                        Menu {
                            id: notificationsMenuSections
                            y: notificationMenuButton.height

                            MenuItem {
                                font.pixelSize: 14
                                text: page.sections[2]
                                onPressed: {
                                    notImplementedDialog.open();
                                }
                            }
                            MenuItem {
                                font.pixelSize: 14
                                text: page.sections[3]
                                onPressed: {
                                    notImplementedDialog.open();
                                }
                            }
                            MenuItem {
                                font.pixelSize: 14
                                text: page.sections[4]
                                onPressed: {
                                    notImplementedDialog.open();
                                }
                            }
                        }
                    }
                }
            }

            Flickable {
                id: scrollview
                Layout.fillHeight: true
                Layout.fillWidth: true
                Layout.alignment: Qt.AlignJustify
                clip: true
                contentWidth: parent.width
                contentHeight: itemGrid.height
                onContentYChanged: {
                    if (scrollview.atYEnd && !page.isBusy) {
                        page.isBusy = true;
                        fillNextReleases();
                        page.isBusy = false;
                    }
                }
                ScrollBar.vertical: ScrollBar {
                    active: true
                }

                ColumnLayout {
                    width: page.width
                    height: page.height
                    Grid {
                        id: itemGrid
                        Layout.alignment: Qt.AlignHCenter
                        columns: 2
                        spacing: 4
                        Repeater {
                            model: page.displayedReleases
                            Rectangle {
                                width: 480
                                height: 260
                                radius: 10
                                border.color: "red"
                                border.width: page.selectedReleases.filter(a => a === modelData.id).length ? 3 : 0
                                color: "#f2f2f2"
                                layer.enabled: true
                                layer.effect: DropShadow {
                                    transparentBorder: true
                                    horizontalOffset: 2
                                    verticalOffset: 2
                                    radius: 1
                                    samples: 3
                                    color: "gray"
                                }
                                MouseArea {
                                    width: 480
                                    height: 260
                                    acceptedButtons: Qt.LeftButton | Qt.RightButton
                                    onClicked: {
                                        if(mouse.button & Qt.RightButton) {
                                            multupleMode.checked = !multupleMode.checked;
                                            return;
                                        }

                                        if (page.openedRelease) return;

                                        page.selectItem(modelData);
                                    }                                    
                                }
                                Grid {
                                    columnSpacing: 3
                                    columns: 2
                                    bottomPadding: 4
                                    leftPadding: 4
                                    topPadding: 4
                                    rightPadding: 4
                                    Image {
                                        source: modelData.poster
                                        fillMode: Image.PreserveAspectCrop
                                        width: 180
                                        height: 252
                                        layer.enabled: true
                                        layer.effect: OpacityMask {
                                            maskSource: mask
                                        }
                                    }
                                    Grid {
                                        height: 260
                                        Layout.row: 1
                                        Layout.column: 1
                                        rows: 2
                                        columns: 1
                                        Column {
                                            id: gridItemtextContainer
                                            Text {
                                                textFormat: Text.RichText
                                                color: "#a32727"
                                                font.pointSize: 12
                                                width: 280
                                                leftPadding: 8
                                                topPadding: 6
                                                wrapMode: Text.WordWrap
                                                maximumLineCount: 3
                                                text: qsTr(modelData.title)
                                            }
                                            Text {
                                                textFormat: Text.RichText
                                                font.pointSize: 10
                                                leftPadding: 8
                                                topPadding: 4
                                                text: qsTr("<b>Статус:</b> ") + qsTr(modelData.status)
                                            }
                                            Text {
                                                font.pointSize: 10
                                                leftPadding: 8
                                                topPadding: 4
                                                text: qsTr("<b>" + modelData.season + " " + modelData.year + "</b>")
                                            }
                                            Text {
                                                textFormat: Text.RichText
                                                font.pointSize: 10
                                                leftPadding: 8
                                                topPadding: 4
                                                width: 280
                                                wrapMode: Text.WordWrap
                                                maximumLineCount: 2
                                                text: qsTr("<b>Тип:</b> ") + qsTr(modelData.type)
                                            }
                                            Text {
                                                font.pointSize: 10
                                                leftPadding: 8
                                                topPadding: 4
                                                width: 280
                                                wrapMode: Text.WordWrap
                                                maximumLineCount: 2
                                                text: qsTr("<b>Жанры:</b> ") + qsTr(modelData.genres)
                                            }
                                            Text {
                                                font.pointSize: 10
                                                leftPadding: 8
                                                topPadding: 4
                                                width: 280
                                                wrapMode: Text.WordWrap
                                                maximumLineCount: 2
                                                text: qsTr("<b>Озвучка:</b> ") + qsTr(modelData.voices)
                                            }
                                        }
                                        Rectangle {
                                            color: "transparent"
                                            height: 252 - gridItemtextContainer.height
                                            width: 280
                                            Row {
                                                anchors.bottom: parent.bottom
                                                leftPadding: 8
                                                topPadding: 8
                                                Image {
                                                    mipmap: true
                                                    source: '../Assets/Icons/rating.svg'
                                                    width: 20
                                                    height: 20
                                                }
                                                Text {
                                                    leftPadding: 4
                                                    rightPadding: 4
                                                    font.pixelSize: 18
                                                    text: modelData.rating
                                                }
                                                ColoredIcon {
                                                    visible: page.favoriteReleases.filter(a => a === modelData.id).length
                                                    iconSource: '../Assets/Icons/star.svg'
                                                    iconWidth: 20
                                                    iconHeight: 20
                                                    iconColor: "#a32727"
                                                }
                                                Text {
                                                    visible: page.favoriteReleases.filter(a => a === modelData.id).length
                                                    leftPadding: 4
                                                    color: "#a32727"
                                                    font.pixelSize: 18
                                                    text: "В избранном"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    ColumnLayout {
        id: cardContainer
        visible: page.openedRelease ? true : false
        anchors.fill: parent
        spacing: 0
        Rectangle {
            color: "#D3D3D3"
            Layout.fillWidth: true
            Layout.fillHeight: true
            Column {
                Grid {
                    id: releaseInfo
                    columnSpacing: 3
                    columns: 3
                    bottomPadding: 4
                    leftPadding: 4
                    topPadding: 4
                    rightPadding: 4
                    Image {
                        id: cardPoster
                        source: page.openedRelease ? page.openedRelease.poster : '../Assets/Icons/donate.jpg'
                        fillMode: Image.PreserveAspectCrop
                        width: 280
                        height: 390
                        layer.enabled: true
                        layer.effect: OpacityMask {
                            maskSource: cardMask
                        }
                    }
                    Column {
                        width: page.width - cardButtons.width - cardPoster.width
                        enabled: !!page.openedRelease
                        Text {
                            textFormat: Text.RichText
                            color: "#a32727"
                            font.pointSize: 12
                            width: parent.width
                            leftPadding: 8
                            topPadding: 6
                            wrapMode: Text.WordWrap
                            maximumLineCount: 3
                            text: qsTr(page.openedRelease ? page.openedRelease.title : '')
                        }
                        Text {
                            textFormat: Text.RichText
                            font.pointSize: 10
                            leftPadding: 8
                            topPadding: 4
                            text: qsTr("<b>Статус:</b> ") + qsTr(page.openedRelease ? page.openedRelease.status : '')
                        }
                        Text {
                            font.pointSize: 10
                            leftPadding: 8
                            topPadding: 4
                            text: qsTr("<b>Год:</b> ") + qsTr(page.openedRelease ? page.openedRelease.year : '')
                        }
                        Text {
                            font.pointSize: 10
                            leftPadding: 8
                            topPadding: 4
                            text: qsTr("<b>Сезон:</b> ") + qsTr(page.openedRelease ? page.openedRelease.season : '')
                        }
                        Text {
                            textFormat: Text.RichText
                            font.pointSize: 10
                            leftPadding: 8
                            topPadding: 4
                            width: parent.width
                            wrapMode: Text.WordWrap
                            maximumLineCount: 2
                            text: qsTr("<b>Тип:</b> ") + qsTr(page.openedRelease ? page.openedRelease.type : '')
                        }
                        Text {
                            font.pointSize: 10
                            leftPadding: 8
                            topPadding: 4
                            width: parent.width
                            wrapMode: Text.WordWrap
                            maximumLineCount: 2
                            text: qsTr("<b>Жанры:</b> ") + qsTr(page.openedRelease ? page.openedRelease.genres : '')
                        }
                        Text {
                            font.pointSize: 10
                            leftPadding: 8
                            topPadding: 4
                            width: parent.width
                            wrapMode: Text.WordWrap
                            maximumLineCount: 2
                            text: qsTr("<b>Озвучка:</b> ") + qsTr(page.openedRelease ? page.openedRelease.voices : '')
                        }
                        Text {
                            textFormat: Text.RichText
                            font.pointSize: 10
                            leftPadding: 8
                            topPadding: 4
                            width: parent.width
                            wrapMode: Text.WordWrap
                            text: qsTr("<b>Описание:</b> ") + qsTr(page.openedRelease ? page.openedRelease.description : '')
                        }
                    }
                    Column {
                        id: cardButtons
                        width: 62
                        IconButton {
                            height: 40
                            width: 40
                            iconColor: "black"
                            iconPath: "../Assets/Icons/close.svg"
                            iconWidth: 28
                            iconHeight: 28
                            onButtonPressed: {
                                page.openedRelease = null;
                            }
                        }
                    }
                }
                Rectangle {
                    color: "transparent"
                    width: cardContainer.width
                    height: 60

                    Button {
                        anchors.verticalCenter: parent.verticalCenter
                        anchors.leftMargin: 10
                        anchors.left: parent.left
                        text: qsTr("Скачать")
                        onClicked: {
                        }
                    }

                    Text {
                        anchors.verticalCenter: parent.verticalCenter
                        anchors.left: parent.left
                        anchors.leftMargin: 100
                        font.pixelSize: 14
                        text: "Доступно "+ (page.openedRelease ? page.openedRelease.countTorrents : "0" ) + " торрентов"
                    }

                    Text {
                        anchors.verticalCenter: parent.verticalCenter
                        anchors.right: parent.right
                        anchors.rightMargin: 100
                        font.pixelSize: 14
                        text: "Доступно "+ (page.openedRelease ? page.openedRelease.countVideos : "0" ) + " серий онлайн"
                    }

                    Button {
                        text: qsTr("Смотреть")
                        anchors.verticalCenter: parent.verticalCenter
                        anchors.right: parent.right
                        anchors.rightMargin: 10
                        onClicked: {
                            watchRelease(page.openedRelease.id, page.openedRelease.videos);
                        }
                    }

                    Row {
                        anchors.verticalCenter: parent.verticalCenter
                        anchors.right: parent.right
                        anchors.rightMargin: 10
                    }

                }
                WebView {
                    id: webView
                    visible: page.openedRelease ? true : false
                    width: cardContainer.width
                    height: cardContainer.height - releaseInfo.height - 60
                    url: page.openedRelease ? "https://vk.com/widget_comments.php?app=5315207&width=100%&_ver=1&limit=8&norealtime=0&url=https://www.anilibria.tv/release/" + page.openedRelease.code + ".html" : "https://vk.com/";
                    onLoadingChanged: {
                        if (loadRequest.errorString) console.error(loadRequest.errorString);
                    }
                    onUrlChanged: {
                        console.log("changed url" + url);
                    }
                }
            }
        }

    }

    MessageDialog {
        id: notImplementedDialog
        title: "Не реализовано"
        text: "Пока указанная функция не реализована"
    }

    function selectItem(item) {
        if (page.selectMode) {
            if (page.openedRelease) page.openedRelease = null;
            if (page.selectedReleases.find(a => a === item.id)) {
                page.selectedReleases = page.selectedReleases.filter(a => a !== item.id);
            } else {
                page.selectedReleases.push(item.id);
            }

            //WORKAROUND: fix refresh list
            const oldSelectedReleases = page.selectedReleases;
            page.selectedReleases = [];
            page.selectedReleases = oldSelectedReleases;
        } else {
            page.openedRelease = item;
        }
    }

    function fillNextReleases() {
        page.pageIndex += 1;
        const newReleases = JSON.parse(localStorage.getReleasesByFilter(page.pageIndex, filterByTitle.textContent, page.selectedSection));
        page.displayedReleases = page.displayedReleases.concat(newReleases);
    }

    function refreshAllReleases() {
        page.pageIndex = 1;
        page.displayedReleases = JSON.parse(localStorage.getReleasesByFilter(page.pageIndex, filterByTitle.textContent, page.selectedSection));
    }

    function changeSection(section) {
        page.selectedSection = section;

        refreshAllReleases();
    }    

    Component.onCompleted: {
        refreshAllReleases();        
    }
}
