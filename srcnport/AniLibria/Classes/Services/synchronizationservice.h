#ifndef SYNCHRONIZATIONSERVICE_H
#define SYNCHRONIZATIONSERVICE_H

#include <QObject>
#include "anilibriaapiservice.h"
#include "../Models/releasemodel.h"

class SynchronizationService : public QObject
{
    Q_OBJECT
private:
    AnilibriaApiService* m_AnilibriaApiService;
    QList<ReleaseModel> m_Releases;
public:    
    explicit SynchronizationService(QObject *parent = nullptr);

    void synchronizeReleases();
signals:

public slots:
    void saveReleasesToCache(QString data);
};

#endif // SYNCHRONIZATIONSERVICE_H
