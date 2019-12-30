import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import QtQuick.Controls.Material 2.0
import "../Controls"

Page {
    id: authorizePage
    property alias email: emailTextBox.text
    property alias password: passwordTextBox.text
    property alias fa2code: fa2codeTextBox.text

    signal navigateFrom()
    signal authentificateFailed(string message)

    onNavigateFrom: {
        emailTextBox.text = "";
        passwordTextBox.text = "";
        fa2codeTextBox.text = "";
    }

    onAuthentificateFailed: {
        errorMessage.text = message;
    }

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
                width: 350
                height: 300

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
                    TextField {
                        id: emailTextBox
                        Layout.fillWidth: true
                        placeholderText: "Email"
                        /*background: Rectangle {
                            color: "whitesmoke"
                            border.width: 1
                            border.color: "red"
                        }*/
                    }
                    Label {
                        Layout.leftMargin: 6
                        font.pixelSize: 12
                        font.bold: true
                        text: qsTr("Пароль")
                    }
                    TextField {
                        id: passwordTextBox
                        Layout.fillWidth: true
                        placeholderText: "Пароль"
                    }
                    Label {
                        Layout.leftMargin: 6
                        font.pixelSize: 12
                        font.bold: true
                        Layout.maximumWidth: authForm.width - 30
                        wrapMode: Text.WordWrap
                        text: qsTr("2fa код (оставить поле пустым если Вы не настроили двухфакторную аутентификацию)")
                    }
                    TextField {
                        id: fa2codeTextBox
                        Layout.fillWidth: true
                        placeholderText: "2fa код"
                    }
                    Rectangle {
                        Layout.fillWidth: true

                        Text {
                            id: errorMessage
                            text: "Error message"
                            anchors.left: parent.left
                            anchors.leftMargin: 10
                        }

                        Button {
                            anchors.right: parent.right
                            anchors.rightMargin: 10
                            text: qsTr("Войти")
                            onClicked: {
                                synchronizationService.authorize(encodeURIComponent(authorizePage.email), encodeURIComponent(authorizePage.password), encodeURIComponent(authorizePage.fa2code));
                            }
                        }

                    }

                }
            }
        }
    }

}
