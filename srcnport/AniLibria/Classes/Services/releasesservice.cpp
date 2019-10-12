#include <QDebug>
#include "releasesservice.h"
#include "../Models/releaseitemmodel.h"
#include "releaseloader.h"

ReleasesService::ReleasesService(QObject *parent) : QObject(parent)
{
    m_NeedUpdate = false;
    m_Releases = *new QVector<ReleaseItemModel*>();
    m_ApiReleases = QList<ReleaseModel>();
    m_ReleaseLoader = new ReleaseLoader();
    m_LoadedReleaseThread = new QThread();

    connect(m_LoadedReleaseThread, SIGNAL(started()), m_ReleaseLoader, SLOT(loadReleases()));
    connect(m_LoadedReleaseThread, &QThread::finished, this, &ReleasesService::loadedReleasesFinished);
    connect(m_ReleaseLoader, &ReleaseLoader::loadedReleases, this, &ReleasesService::loadedReleases);
    m_ReleaseLoader->moveToThread(m_LoadedReleaseThread);
}

void ReleasesService::fillNextReleases()
{
    int startPosition = m_Releases.count();
    int iterator = 0;

    for (int i = startPosition; i < m_ApiReleases.count(); i++) {
        if (iterator >= 20) break;

        ReleaseItemModel * releaseItemModel = new ReleaseItemModel();
        releaseItemModel->mapFromReleaseModel(m_ApiReleases[i]);
        m_Releases.append(releaseItemModel);

        iterator++;
    }

    emit releasesChanged();
}

void ReleasesService::loadReleasesCache()
{
    if (m_LoadedReleaseThread->isRunning()) {
        m_NeedUpdate = true;
        return;
    }

    m_LoadedReleaseThread->start();
}

ReleaseItemModel *ReleasesService::getRelease(int releaseId)
{
    ReleaseItemModel * findRelease = nullptr;
    foreach(ReleaseItemModel *release, m_Releases) {
        if (release->id() == releaseId) {
            findRelease = release;
            break;
        }
    }
    return findRelease;
}

QQmlListProperty<ReleaseItemModel> ReleasesService::releases()
{
    return QQmlListProperty<ReleaseItemModel>(
        this,
        this,
        &ReleasesService::addRelease,
        &ReleasesService::releasesCount,
        &ReleasesService::release,
        &ReleasesService::clearReleases
    );
}

void ReleasesService::addRelease(ReleaseItemModel * release)
{
    m_Releases.append(release);
}

int ReleasesService::releasesCount() const
{
    return m_Releases.count();
}

ReleaseItemModel *ReleasesService::release(int index) const
{
    return m_Releases.at(index);
}

void ReleasesService::clearReleases()
{
    m_Releases.clear();
}

void ReleasesService::loadedReleases()
{
    m_LoadedReleaseThread->quit();
    m_ApiReleases = m_ReleaseLoader->releases();
    fillNextReleases();
}

void ReleasesService::loadedReleasesFinished()
{
    if (m_NeedUpdate) loadReleasesCache();
}

void ReleasesService::addRelease(QQmlListProperty<ReleaseItemModel> * list, ReleaseItemModel * release)
{
    reinterpret_cast<ReleasesService*>(list->data)->addRelease(release);
}

int ReleasesService::releasesCount(QQmlListProperty<ReleaseItemModel> * list)
{
    return reinterpret_cast<ReleasesService*>(list->data)->releasesCount();
}

ReleaseItemModel *ReleasesService::release(QQmlListProperty<ReleaseItemModel> * list, int index)
{
    return reinterpret_cast<ReleasesService*>(list->data)->release(index);
}

void ReleasesService::clearReleases(QQmlListProperty<ReleaseItemModel> * list)
{
    reinterpret_cast<ReleasesService*>(list->data)->clearReleases();
}

