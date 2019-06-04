import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import QtQuick.Controls.Material 2.0
import QtGraphicalEffects 1.12
import Anilibria.Services 1.0
import "Views"

ApplicationWindow {
    id: window
    visible: true
    width: 800
    minimumWidth: 800
    minimumHeight: 600
    height: 600
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
        //currentPage.navigateFrom();

        currentPage.visible = false;

        const newPage = pages[pageId];
        newPage.visible = true;
        currentPageId = pageId;

        drawer.close();
    }

    SynchronizationService {
        id: synchronizationService
        Component.onCompleted: {
            synchronizationService.synchronizeReleases();
        }
    }

    //TODO: Switch beetween themes
    //Material.theme: Material.Dark

    Drawer {
        id: drawer
        width: window.width * 0.4
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
        drawer: drawer
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
        drawer: drawer
        visible: true
    }

    Authorization {
        id: authorization
        drawer: drawer
        visible: false
    }

    Donate {
        id: donate
        drawer: drawer
        visible: false
    }

    Youtube {
        id: youtube
        drawer: drawer
        visible: false
    }
}
