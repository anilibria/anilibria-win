#include <QtCore>
#include <QQmlListProperty>
#include "synchronizationservice.h"
#include "../Models/releasemodel.h"
#include "../Models/releaseitemmodel.h"

SynchronizationService::SynchronizationService(QObject *parent) : QObject(parent)
{
    m_AnilibriaApiService = new AnilibriaApiService(this);
    m_Releases = *new QVector<ReleaseItemModel*>();
    m_ApiReleases = QList<ReleaseModel>();
    connect(m_AnilibriaApiService,SIGNAL(allReleasesReceived(QString)),this,SLOT(saveReleasesToCache(QString)));
}

void SynchronizationService::synchronizeReleases()
{
    m_AnilibriaApiService->getAllReleases();
}

QQmlListProperty<ReleaseItemModel> SynchronizationService::releases()
{
    return QQmlListProperty<ReleaseItemModel>(
        this,
        this,
        &SynchronizationService::addRelease,
        &SynchronizationService::releasesCount,
        &SynchronizationService::release,
        &SynchronizationService::clearReleases
    );
}

void SynchronizationService::addRelease(ReleaseItemModel * release)
{
    m_Releases.append(release);
}

int SynchronizationService::releasesCount() const
{
    return m_Releases.count();
}

ReleaseItemModel *SynchronizationService::release(int index) const
{
    return m_Releases.at(index);
}

void SynchronizationService::clearReleases()
{
    m_Releases.clear();
}

void SynchronizationService::addRelease(QQmlListProperty<ReleaseItemModel> * list, ReleaseItemModel * release)
{
    reinterpret_cast<SynchronizationService*>(list->data)->addRelease(release);
}

int SynchronizationService::releasesCount(QQmlListProperty<ReleaseItemModel> * list)
{
    return reinterpret_cast<SynchronizationService*>(list->data)->releasesCount();
}

ReleaseItemModel *SynchronizationService::release(QQmlListProperty<ReleaseItemModel> * list, int index)
{
    return reinterpret_cast<SynchronizationService*>(list->data)->release(index);
}

void SynchronizationService::clearReleases(QQmlListProperty<ReleaseItemModel> * list)
{
    reinterpret_cast<SynchronizationService*>(list->data)->clearReleases();
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
    QJsonValue payload = rootObject["data"];
    QJsonArray items = payload["items"].toArray();
    foreach(const QJsonValue & item, items) {
        ReleaseModel releaseModel = ReleaseModel();
        releaseModel.readFromApiModel(item.toObject());
        m_ApiReleases.append(releaseModel);
    }

    //TODO: save releases to cache

    ReleaseModel item;
    int iterator = 0;
    foreach (item, m_ApiReleases) {
        ReleaseItemModel * releaseItemModel = new ReleaseItemModel();
        releaseItemModel->mapFromReleaseModel(item);
        m_Releases.append(releaseItemModel);

        if (iterator > 50) break; // maximum of page 50 items
    }
    emit releasesChanged();
}
