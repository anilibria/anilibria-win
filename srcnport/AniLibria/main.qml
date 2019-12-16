import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import QtQuick.Controls.Material 2.0
import QtGraphicalEffects 1.12
import Anilibria.Services 1.0
import "Views"
import "./anilibriaService.js" as AnilibriaService

ApplicationWindow {
    id: window
    visible: true
    width: 800
    minimumWidth: 800
    minimumHeight: 600
    height: 600
    //flags: Qt.FramelessWindowHint | Qt.WindowMinimizeButtonHint | Qt.Window
    title: qsTr("AniLibria")
    property string currentPageId: "release"

    function showPage(pageId) {
        if (currentPageId === pageId){
            drawer.close();
            return;
        }

        const pages = {
            "videoplayer": videoplayer,
            "authorization": authorization,
            "release": releases,
            "donate": donate,
            "youtube": youtube
        };

        const currentPage = pages[currentPageId];
        currentPage.navigateFrom();

        currentPage.visible = false;

        const newPage = pages[pageId];
        newPage.visible = true;
        currentPageId = pageId;

        drawer.close();
    }

    LocalStorage {
        id: localStorage

    }

    SynchronizationService {
        id: synchronizationService
        Component.onCompleted: {
            synchronizationService.synchronizeReleases();
        }
        onSynchronizationCompleted: {
            //releasesService.loadReleasesCache();
        }
        onSynchronizedReleases: {
            const jsonData = JSON.parse(data);

            if (!jsonData.status) {
                //TODO: handle error situation
            }

            const jsonReleases = jsonData.data.items;
            localStorage.updateAllReleases(JSON.stringify(jsonReleases));

            releases.refreshAllReleases();
        }
    }

    ReleasesService {
        id: releasesService
        Component.onCompleted: {
            releasesService.loadReleasesCache();
        }
    }

    Drawer {
        id: drawer
        width: 300
        height: window.height
        background:  LinearGradient {
            anchors.fill: parent
            start: Qt.point(0, 0)
            end: Qt.point(0, 300)
            gradient: Gradient {
                GradientStop { position: 0.0; color: "#6b0200" }
                GradientStop { position: 0.3; color: "#ab0909" }
                GradientStop { position: 0.5; color: "#d41515" }
                GradientStop { position: 0.7; color: "#fc2626" }
                GradientStop { position: 1.0; color: "#f52c39" }
            }
        }

        Column {
            anchors.fill: parent

            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/menu.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Каталог релизов")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("release");
                }
            }
            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/videoplayer.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Видеоплеер")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("videoplayer");
                }
            }
            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/youtube.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Youtube")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("youtube");
                }
            }
            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/donate.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Поддержать проект")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("donate");
                }
            }
            ItemDelegate {
                contentItem: Item {
                    Row {
                        spacing: 10
                        Image {
                            source: "Assets/Icons/user.svg"
                            sourceSize.width: 30
                            sourceSize.height: 30
                        }
                        Label {
                            anchors.leftMargin: 10
                            anchors.verticalCenter: parent.verticalCenter
                            color: "white"
                            text: qsTr("Войти")
                        }
                    }
                }
                width: parent.width
                onClicked: {
                    showPage("authorization");
                }
            }
        }
    }

    OnlinePlayer {
        id: videoplayer
        visible: false
        onChangeFullScreenMode: {
            if (fullScreen) {
                window.showFullScreen();
            } else {
                window.showNormal();
            }
        }
    }

    Releases {
        id: releases
        visible: true
        onWatchRelease: {
            videoplayer.setReleaseVideo(releaseId, -1);
            window.showPage("videoplayer");
        }
    }

    Authorization {
        id: authorization
        visible: false
    }

    Donate {
        id: donate
        visible: false
    }

    Youtube {
        id: youtube
        drawer: drawer
        releasesService: releasesService
        visible: false
    }
}
