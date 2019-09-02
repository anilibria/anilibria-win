#include <QtCore>
#include <QQmlListProperty>
#include "synchronizationservice.h"
#include "../Models/releasemodel.h"
#include "../Models/releaseitemmodel.h"

SynchronizationService::SynchronizationService(QObject *parent) : QObject(parent)
{
    m_AnilibriaApiService = new AnilibriaApiService(this);
    connect(m_AnilibriaApiService,SIGNAL(allReleasesReceived(QString)),this,SLOT(saveReleasesToCache(QString)));
}

void SynchronizationService::synchronizeReleases()
{
    m_AnilibriaApiService->getAllReleases();
}

void SynchronizationService::saveReleasesToCache(QString data)
{
    QJsonParseError jsonError;
    QJsonDocument doc = QJsonDocument::fromJson(data.toUtf8(), &jsonError);
    if (jsonError.error != QJsonParseError::NoError){
        //qDebug() << jsonError.errorString();
        //TODO: need handle this situation
    }

    QJsonObject rootObject = doc.object();
    bool status = rootObject["status"].toBool();
    if (!status) {
        //TODO: need handle this situation
        //read from error object
    }
    auto ApiReleases = QList<ReleaseModel>();

    QJsonValue payload = rootObject["data"];
    QJsonArray items = payload["items"].toArray();
    foreach(const QJsonValue & item, items) {
        ReleaseModel releaseModel = ReleaseModel();
        releaseModel.readFromApiModel(item.toObject());
        ApiReleases.append(releaseModel);
    }

    auto path = QStandardPaths::writableLocation(QStandardPaths::AppLocalDataLocation) + "/save.json";
    QFile saveFile(path);

    if (!saveFile.open(QIODevice::WriteOnly)) {
        qWarning("Couldn't open save file.");
        return;
    }

    QJsonArray releasesJson;
    foreach(const ReleaseModel & release, ApiReleases) {
        QJsonObject jsonObject;
        release.writeToJson(jsonObject);
        releasesJson.append(jsonObject);
    }
    QJsonDocument saveDoc(releasesJson);
    saveFile.write(saveDoc.toJson());

    emit synchronizationCompleted();
}
