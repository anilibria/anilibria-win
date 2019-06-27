import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import QtGraphicalEffects 1.0
import Anilibria.Services 1.0

Page {
    id: page
    property Drawer drawer
    property bool isBusy: false
    property SynchronizationService synchronizeService
    signal navigateFrom()

    onWidthChanged: {
        const columnCount = parseInt(page.width / 400);
        itemGrid.columns = columnCount < 1 ? 1 : columnCount;
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

    Flickable {
        id: scrollview
        anchors.fill: parent
        contentWidth: parent.width
        contentHeight: itemGrid.height
        onContentYChanged: {
            if (scrollview.atYEnd && !page.isBusy) {
                page.isBusy = true;
                synchronizationService.fillNextReleases();
                page.isBusy = false;
            }
        }
        ScrollBar.vertical: ScrollBar {
            active: true;

            /*onActiveChanged: {
                if (!active) active = true;
            }*/
        }

        ColumnLayout {
            width: page.width
            height: page.height
            Grid {
                id: itemGrid
                Layout.alignment: Qt.AlignHCenter
                columns: 2
                spacing: 4
                //width: 540
                Repeater {
                    model: synchronizeService.releases
                    Rectangle {
                        width: 460
                        height: 260
                        radius: 10
                        color: "#f2f2f2"
                        Row {
                            Image {
                                source: modelData.poster
                                fillMode: Image.PreserveAspectCrop //Image.PreserveAspectFit
                                width: 180
                                height: 260
                                layer.enabled: true
                                layer.effect: OpacityMask {
                                    maskSource: mask
                                }
                            }

                            Column {
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
                                    text: qsTr("<b>Год:</b> ") + qsTr(modelData.year)
                                }
                                Text {
                                    textFormat: Text.RichText
                                    font.pointSize: 10
                                    leftPadding: 8
                                    topPadding: 4
                                    width: 280
                                    wrapMode: Text.WordWrap
                                    maximumLineCount: 2
                                    text: qsTr("<b>Тип:</b> ") + qsTr(modelData.releaseType)
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
                                    text: qsTr("<b>Озвучка:</b> ") + qsTr(modelData.voicers)
                                }
                            }
                        }
                    }
                }
            }
        }
    }

}

