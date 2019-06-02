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

private:
    QString m_Title;
    QString m_Status;
    QString m_Year;
    QString m_Poster;

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

    void mapFromReleaseModel(ReleaseModel & releaseModel);

signals:
    void titleChanged();
    void statusChanged();
    void yearChanged();
    void posterChanged();

public slots:

};

#endif // RELEASEITEMMODEL_H
