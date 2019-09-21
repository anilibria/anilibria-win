#include "releasesservice.h"
#include "../Models/releaseitemmodel.h"

ReleasesService::ReleasesService(QObject *parent) : QObject(parent)
{
    m_Releases = *new QVector<ReleaseItemModel*>();
    m_ApiReleases = QList<ReleaseModel>();
}

void ReleasesService::fillNextReleases()
{
    int startPosition = m_Releases.count();
    int iterator = 0;

    for (int i = startPosition; i < m_ApiReleases.count(); i++) {
        if (iterator >= 30) break;

        ReleaseItemModel * releaseItemModel = new ReleaseItemModel();
        releaseItemModel->mapFromReleaseModel(m_ApiReleases[i]);
        m_Releases.append(releaseItemModel);

        iterator++;
    }

    emit releasesChanged();
}

void ReleasesService::loadReleasesCache()
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

    m_ApiReleases.clear();

    foreach(const QJsonValue & release, releasesArray) {
        auto releaseModel = ReleaseModel();
        releaseModel.readFromJson(release.toObject());
        m_ApiReleases.append(releaseModel);
    }

    fillNextReleases();
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

