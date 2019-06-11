import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import QtQuick.Controls.Material 2.0
import "../Controls"

Page {
    property Drawer drawer
    signal navigateFrom()

    background: Rectangle {
        color: "#D3D3D3"
    }

    anchors.fill: parent

    RowLayout {
        id: panelContainer
        anchors.fill: parent
        spacing: 0
        Rectangle {
            color: "#9e2323"
            width: 40
            Layout.fillHeight: true
            Column {
                AppPanelButton {
                    iconSource: "../Assets/Icons/menu.svg"
                    width: panelContainer.width
                    onPressed: {
                        drawer.open();
                    }
                }
            }
        }
        Rectangle {
            color: "#D3D3D3"
            Layout.fillHeight: true
            Layout.fillWidth: true

            Rectangle {
                radius: 8
                anchors.verticalCenter: parent.verticalCenter
                anchors.horizontalCenter: parent.horizontalCenter
                width: parent.width / 2
                height: 240

                ColumnLayout {
                    id: authForm
                    anchors.fill: parent
                    anchors.margins: 8
                    Label {
                        Layout.leftMargin: 6
                        font.pixelSize: 12
                        font.bold: true
                        text: qsTr("Email")
                    }
                    RoundedTextBox {
                        Layout.fillWidth: true
                        textContent: "Bluherka"
                    }
                    Label {
                        Layout.leftMargin: 6
                        font.pixelSize: 12
                        font.bold: true
                        text: qsTr("Пароль")
                    }
                    RoundedTextBox {
                        Layout.fillWidth: true
                        textContent: "password"
                    }
                    Label {
                        Layout.leftMargin: 6
                        font.pixelSize: 12
                        font.bold: true
                        Layout.maximumWidth: authForm.width - 30
                        wrapMode: Text.WordWrap
                        text: qsTr("2fa код (оставить поле пустым если Вы не настроили двухфакторную аутентификацию)")
                    }
                    RoundedTextBox {
                        Layout.fillWidth: true
                        textContent: "2fa"
                    }
                    Button {
                        Layout.alignment: Qt.AlignRight
                        text: qsTr("Войти")
                        onClicked: {
                            drawer.open();
                        }
                    }
                    /*Button {
                        text: qsTr("Открыть сайдбар")
                        onClicked: {
                            drawer.open();
                        }
                    }*/
                }
            }
        }
    }

}
