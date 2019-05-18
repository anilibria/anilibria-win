#include <QtCore>
#include "synchronizationservice.h"
#include "../Models/releasemodel.h"

SynchronizationService::SynchronizationService(QObject *parent) : QObject(parent)
{
    m_AnilibriaApiService = new AnilibriaApiService(this);
    m_Releases = QList<ReleaseModel>();
    connect(m_AnilibriaApiService,SIGNAL(allReleasesReceived(QString)),this,SLOT(saveReleasesToCache(QString)));
}

void SynchronizationService::synchronizeReleases()
{
    m_AnilibriaApiService->getAllReleases();
}

/*
"id":1202,
    "code":"sakurako-san-no-ashimoto-ni-wa-shitai-ga-umatteiru",
    "names":[
        "Труп под ногами Сакурако",
        "Sakurako-san no Ashimoto ni wa Shitai ga Umatteiru"
    ],
    "series":"1-12",
    "poster":"/upload/release/350x500/default.jpg",
    "favorite": МОДЕЛЬ ИЗБРАННОГО В РЕЛИЗЕ,
    "last":"1202",
    "moon":"https://streamguard.cc/serial/f9f3c92e182de8c722ed0c13e8087558/iframe?nocontrols_translations=1",
    "status":"Завершен",
    "type":"ТВ (>12 эп.), 25 мин.",
    "genres":[
        "приключения",
        "мистика",
        "детектив"
    ],
    "voices":[
        "Mikrobelka",
        "HectoR",
        "Aemi"
    ],
    "year":"0",
    "day":"1",
    "description":"Описание релиза <a href='#'>которое может содержать html</a>",
    "blockedInfo": МОДЕЛЬ БЛОКИРОВКИ,
    "playlist":[ МОДЕЛЬ СЕРИИ ],
    "torrents":[ МОДЕЛЬ ТОРРЕНТА]
*/

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
    QJsonValue payload = rootObject["data"];
    QJsonArray items = payload["items"].toArray();
    foreach(const QJsonValue & item, items) {
        ReleaseModel releaseModel = ReleaseModel();
        releaseModel.readFromApiModel(item.toObject());
        m_Releases.append(releaseModel);
    }
}
