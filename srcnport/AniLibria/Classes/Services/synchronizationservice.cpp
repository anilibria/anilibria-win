#include <QtCore>
#include <QQmlListProperty>
#include "synchronizationservice.h"
#include "../Models/releasemodel.h"

SynchronizationService::SynchronizationService(QObject *parent) : QObject(parent)
{
    m_AnilibriaApiService = new AnilibriaApiService(this);
    connect(m_AnilibriaApiService,&AnilibriaApiService::allReleasesReceived,this,&SynchronizationService::saveReleasesToCache);
    connect(m_AnilibriaApiService,&AnilibriaApiService::scheduleReceived,this,&SynchronizationService::saveScheduleToCache);
}

void SynchronizationService::synchronizeReleases()
{
    m_AnilibriaApiService->getAllReleases();
}

void SynchronizationService::synchronizeSchedule()
{
    m_AnilibriaApiService->getSchedule();
}

void SynchronizationService::saveReleasesToCache(QString data)
{
    emit synchronizedReleases(data);
}

void SynchronizationService::saveScheduleToCache(QString data)
{
    emit synchronizedSchedule(data);
}
