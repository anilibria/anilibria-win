#ifndef RELEASEITEMMODEL_H
#define RELEASEITEMMODEL_H

#include <QObject>
#include "releasemodel.h"

class ReleaseItemModel : public QObject
{
    Q_OBJECT

    Q_PROPERTY(QString title READ title WRITE setTitle NOTIFY titleChanged)
    Q_PROPERTY(QString status READ status WRITE setStatus NOTIFY statusChanged)
    Q_PROPERTY(QString year READ year WRITE setYear NOTIFY yearChanged)
    Q_PROPERTY(QString poster READ poster WRITE setPoster NOTIFY posterChanged)
    Q_PROPERTY(QString description READ description WRITE setDescription NOTIFY descriptionChanged)
    Q_PROPERTY(QString releaseType READ releaseType WRITE setReleaseType NOTIFY releaseTypeChanged)

private:
    QString m_Title;
    QString m_Status;
    QString m_Year;
    QString m_Poster;
    QString m_Description;
    QString m_ReleaseType;

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

    void mapFromReleaseModel(ReleaseModel & releaseModel);

signals:
    void titleChanged();
    void statusChanged();
    void yearChanged();
    void posterChanged();
    void descriptionChanged();
    void releaseTypeChanged();

public slots:

};

#endif // RELEASEITEMMODEL_H