#ifndef RELEASEITEMMODEL_H
#define RELEASEITEMMODEL_H

#include <QObject>
#include <QQmlListProperty>
#include "releasemodel.h"
#include "releasevideomodel.h"

class ReleaseItemModel : public QObject
{
    Q_OBJECT

    Q_PROPERTY(QString title READ title WRITE setTitle NOTIFY titleChanged)
    Q_PROPERTY(QString status READ status WRITE setStatus NOTIFY statusChanged)
    Q_PROPERTY(QString year READ year WRITE setYear NOTIFY yearChanged)
    Q_PROPERTY(QString poster READ poster WRITE setPoster NOTIFY posterChanged)
    Q_PROPERTY(QString description READ description WRITE setDescription NOTIFY descriptionChanged)
    Q_PROPERTY(QString releaseType READ releaseType WRITE setReleaseType NOTIFY releaseTypeChanged)
    Q_PROPERTY(QString genres READ genres WRITE setGenres NOTIFY genresChanged)
    Q_PROPERTY(QString voicers READ voicers WRITE setVoicers NOTIFY voicersChanged)
    Q_PROPERTY(QString season READ season WRITE setSeason NOTIFY seasonChanged)
    Q_PROPERTY(int id READ id WRITE setId NOTIFY idChanged)
    Q_PROPERTY(bool selected READ selected WRITE setSelected NOTIFY selectedChanged)
    Q_PROPERTY(QQmlListProperty<ReleaseVideoModel> videos READ videos)
    Q_PROPERTY(int countOnlineVideos READ countOnlineVideos WRITE setCountOnlineVideos NOTIFY countOnlineVideosChanged)

private:
    QString m_Title;
    QString m_Status;
    QString m_Year;
    QString m_Poster;
    QString m_Description;
    QString m_ReleaseType;
    QString m_Genres;
    QString m_Voicers;
    QString m_Season;
    int m_CountOnlineVideos;
    int m_Id;
    bool m_Selected;
    QList<ReleaseVideoModel*> m_Videos;

public:
    explicit ReleaseItemModel(QObject *parent = nullptr);

    QString title() const;
    void setTitle(const QString &title);

    QString status() const;
    void setStatus(const QString &status);

    QString year() const;
    void setYear(const QString &year);

    QString poster() const;
    void setPoster(const QString &poster);

    QString description() const;
    void setDescription(const QString &description);

    QString releaseType() const;
    void setReleaseType(const QString &releaseType);

    QString genres() const;
    void setGenres(const QString &genres);

    QString voicers() const;
    void setVoicers(const QString &voicers);

    bool selected() const;
    void setSelected(const bool selected);

    QString season() const;
    void setSeason(const QString &season);

    int id() const;
    void setId(const int id);

    int countOnlineVideos() const;
    void setCountOnlineVideos(const int countOnlineVideos);

    QQmlListProperty<ReleaseVideoModel> videos();

    void mapFromReleaseModel(ReleaseModel & releaseModel);

signals:
    void titleChanged();
    void statusChanged();
    void yearChanged();
    void posterChanged();
    void descriptionChanged();
    void releaseTypeChanged();
    void genresChanged();
    void voicersChanged();
    void selectedChanged();
    void seasonChanged();
    void idChanged();
    void countOnlineVideosChanged();

public slots:

};

#endif // RELEASEITEMMODEL_H
