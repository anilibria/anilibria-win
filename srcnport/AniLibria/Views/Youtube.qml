import QtQuick 2.12
import QtQuick.Controls 2.5
import QtQuick.Layouts 1.3
import QtGraphicalEffects 1.0
import Anilibria.Services 1.0

Page {
    id: page
    property bool isBusy: false
    signal navigateFrom()

    /*onWidthChanged: {
        const columnCount = parseInt(page.width / 500);
        itemGrid.columns = columnCount < 1 ? 1 : columnCount;
    }*/

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


}

