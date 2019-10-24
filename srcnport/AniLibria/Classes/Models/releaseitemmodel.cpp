#include <QtCore>
#include "releaseitemmodel.h"
#include "releasevideomodel.h"

void ReleaseItemModel::mapFromReleaseModel(ReleaseModel &releaseModel)
{
    setSelected(false);
    setTitle(releaseModel.title());
    setStatus(releaseModel.status());
    setYear(releaseModel.year());
    QString fullPosterUrl = "https://www.anilibria.tv" + releaseModel.poster();
    setPoster(fullPosterUrl);
    setReleaseType(releaseModel.type());
    setVoicers(releaseModel.voices().join(", "));
    setGenres(releaseModel.genres().join(", "));
    setDescription(releaseModel.description());
    setSeason(releaseModel.season());
    setId(releaseModel.id());
    QList<OnlineVideoModel> releaseVideos = releaseModel.videos();
    foreach(OnlineVideoModel video, releaseVideos) {
        auto videoModel = new ReleaseVideoModel(this);
        videoModel->setId(video.id());
        videoModel->setSd(video.sd());
        videoModel->setHd(video.hd());
        videoModel->setFullhd(video.fullhd());
        videoModel->setSrchd(video.sourcehd());
        videoModel->setSrcsd(video.sourcesd());
        videoModel->setTitle(video.title());
        m_Videos.append(videoModel);
    }
    setCountOnlineVideos(releaseVideos.length());
}

ReleaseItemModel::ReleaseItemModel(QObject *parent) : QObject(parent)
{
    m_Videos = QList<ReleaseVideoModel*>();
}

QString ReleaseItemModel::title() const
{
    return m_Title;
}

void ReleaseItemModel::setTitle(const QString &title)
{
    if (title == m_Title) return;

    m_Title = title;
    emit titleChanged();
}

QString ReleaseItemModel::status() const
{
    return m_Status;
}

void ReleaseItemModel::setStatus(const QString &status)
{
    if (status == m_Status) return;

    m_Status = status;
    emit statusChanged();
}

QString ReleaseItemModel::year() const
{
    return m_Year;
}

void ReleaseItemModel::setYear(const QString &year)
{
    if (year == m_Year) return;

    m_Year = year;
    emit yearChanged();
}

QString ReleaseItemModel::poster() const
{
    return m_Poster;
}

void ReleaseItemModel::setPoster(const QString &poster)
{
    if (poster == m_Poster) return;

    m_Poster = poster;
    emit posterChanged();
}

QString ReleaseItemModel::description() const
{
    return m_Description;
}

void ReleaseItemModel::setDescription(const QString &description)
{
    if (description == m_Description) return;

    m_Description = description;
    emit descriptionChanged();
}

QString ReleaseItemModel::releaseType() const
{
    return m_ReleaseType;
}

void ReleaseItemModel::setReleaseType(const QString &releaseType)
{
    if (releaseType == m_ReleaseType) return;

    m_ReleaseType = releaseType;
    emit releaseTypeChanged();
}

QString ReleaseItemModel::genres() const
{
    return m_Genres;
}

void ReleaseItemModel::setGenres(const QString &genres)
{
    if (genres == m_Genres) return;

    m_Genres = genres;
    emit genresChanged();
}

QString ReleaseItemModel::voicers() const
{
    return m_Voicers;
}

void ReleaseItemModel::setVoicers(const QString &voicers)
{
    if (voicers == m_Voicers) return;

    m_Voicers = voicers;
    emit voicersChanged();
}

bool ReleaseItemModel::selected() const
{
    return m_Selected;
}

void ReleaseItemModel::setSelected(const bool selected)
{
    if (selected == m_Selected) return;

    m_Selected = selected;
    emit selectedChanged();
}

QString ReleaseItemModel::season() const
{
    return m_Season;
}

void ReleaseItemModel::setSeason(const QString &season)
{
    if (season == m_Season) return;

    m_Season = season;
    emit seasonChanged();
}

int ReleaseItemModel::id() const
{
    return m_Id;
}

void ReleaseItemModel::setId(const int id)
{
    if (id == m_Id) return;

    m_Id = id;
    emit idChanged();
}

int ReleaseItemModel::countOnlineVideos() const
{
    return m_CountOnlineVideos;
}

void ReleaseItemModel::setCountOnlineVideos(const int countOnlineVideos)
{
    if (countOnlineVideos == m_CountOnlineVideos) return;

    m_CountOnlineVideos = countOnlineVideos;
    emit countOnlineVideosChanged();
}

QQmlListProperty<ReleaseVideoModel> ReleaseItemModel::videos()
{
    return QQmlListProperty<ReleaseVideoModel>(this, m_Videos);
}
