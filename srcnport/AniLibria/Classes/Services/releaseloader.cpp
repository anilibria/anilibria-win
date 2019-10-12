#include <QtCore>
#include "releaseloader.h"
#include "../Models/releaseitemmodel.h"

ReleaseLoader::ReleaseLoader(QObject *parent) : QObject(parent)
{

}

QList<ReleaseModel> ReleaseLoader::releases() const
{
    return *list;
}

void ReleaseLoader::loadReleases()
{
    auto path = QStandardPaths::writableLocation(QStandardPaths::AppLocalDataLocation) + "/save.json";
    if (!QFileInfo::exists(path)) return;

    QFile file(path);

    if (!file.open(QIODevice::ReadOnly)) {
        qWarning("Couldn't open save file.");
        return;
    }

    QByteArray saveData = file.readAll();
    file.close();

    QJsonDocument releasesDocument(QJsonDocument::fromJson(saveData));
    auto releasesArray = releasesDocument.array();

    list->clear();

    foreach(const QJsonValue & release, releasesArray) {
        auto releaseModel = ReleaseModel();
        releaseModel.readFromJson(release.toObject());
        list->append(releaseModel);
    }

    emit loadedReleases();
}
